using IMonitorPluginBase;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross_Platform_System_Monitor.Core
{
    public class MonitorSystemInfoService : BackgroundService
    {
        private ISystemMetricsProvider systemMetricsProvider;
        private IEnumerable<IMonitorPlugin> monitorPlugins;
        private Config config;

        // Constructor injection of dependencies (monitorPlugin,systemMetricsProvider
        public MonitorSystemInfoService(ISystemMetricsProvider systemMetricProvider,
            IEnumerable<IMonitorPlugin> monitorPlugins,IOptions<Config> options)
        {
            this.systemMetricsProvider = systemMetricProvider;
            this.monitorPlugins = monitorPlugins;
            this.config = options.Value;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (true)
                {
                    SystemMetrics updatedSystemMetrics = systemMetricsProvider.GetSystemMetrics();

                    Console.WriteLine($"[MonitorSystemInfoService] Retrieved System Metrics at {updatedSystemMetrics.ToString()}");

                    var pluginTasks = monitorPlugins.Select(plugin => Task.Run(() =>
                    {
                        try
                        {
                            plugin.Execute(updatedSystemMetrics);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\n[MonitorSystemInfoService] Error executing plugin {plugin.Name}: {ex.Message}");
                        }
                    }, stoppingToken)).ToArray();

                    await Task.WhenAll(pluginTasks);

                    await Task.Delay(config.Monitoring.IntervalSeconds * 1000, stoppingToken);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in MonitorSystemInfoService: " + ex.Message);
            }
            
            
        }
    }
}
