using Cross_Platform_System_Monitor.Core;
using Cross_Platform_System_Monitor.Platform;
using IMonitorPluginBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Threading.Tasks;

namespace Cross_Platform_System_Monitor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Config>(context.Configuration);

                    if (OperatingSystem.IsWindows())
                    {
                        services.AddSingleton<ISystemMetricsProvider, WindowSystemMetricsProvider>();
                    }
                    else
                    {
                        Console.WriteLine("Using Other Platform System Metrics Provider:: Currently Unavailable !!!");
                    }
                    services.AddHostedService<MonitorSystemInfoService>();

                    IEnumerable<IMonitorPlugin> plugins = GetPlugins();
                    var config = context.Configuration.Get<Config>();
                    foreach (var plugin in plugins)
                    {
                        services.AddSingleton<IMonitorPlugin>(plugin);
                        plugin.Configure(config);
                    }
                })
                .Build();
                await host.RunAsync();

            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in Running the host: " + ex.Message);
            }

        }
        public static Assembly LoadPlugin(string assemblyPath)
        {

            Console.WriteLine("Loading plugin from: " + assemblyPath);
            var loadContext = new PluginLoadContext(assemblyPath);

            return loadContext.LoadFromAssemblyPath(assemblyPath);

        }
        public static List<IMonitorPlugin> GetPlugins()
        {
            try
            {
                var binDir = Environment.CurrentDirectory + "/Plugins";
                var files = Directory.GetFiles(binDir, "*.dll").ToList();

                files.Remove(typeof(Program).Assembly.Location);
                files.Remove(Path.Combine(binDir, "IMonitorPluginBase.dll"));

                var commands = files.SelectMany(pluginPath =>
                {
                    var pluginAssembly = LoadPlugin(pluginPath);
                    return CreateCommands(pluginAssembly);

                }).ToList();
                return commands;

            }
            catch(Exception ex)
            {
                Console.WriteLine("Error loading plugins: " + ex.Message);
                return new List<IMonitorPlugin>();
            }
            
        }
        
        static IEnumerable<IMonitorPlugin> CreateCommands(Assembly pluginAssembly)
        {

            int count = 0;
            foreach(var type in pluginAssembly.GetTypes())
            {
                if(type is not null)
                {
                    if(type.GetInterfaces().Any(intf => intf.FullName?.Contains(nameof(IMonitorPluginBase.IMonitorPlugin)) ?? false))
                    {
                        var result = Activator.CreateInstance(type) as IMonitorPlugin;
                        if(result != null)
                        {
                            count++;
                            yield return result;
                        }
                    }

                }
                
            }
            if(count == 0)
            {
                var availableTypes = string.Join(", ", pluginAssembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"No valid IMonitorPlugin implementations found in assembly {pluginAssembly.FullName}. Available types: {availableTypes}");
            }
            
        }

    }    

}
