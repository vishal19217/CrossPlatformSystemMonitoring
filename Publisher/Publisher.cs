using IMonitorPluginBase;
using System.Net.Http.Json;

namespace Publisher
{
    public class Publisher : IMonitorPlugin
    {
        private Config? config;

        HttpClient? client;
        public string Name
        {
            get { return "Metric Publisher API Plugin"; }
        }

        public string Description
        {
            get { return "Publishes System Metrics to the API"; }
        }


        public void Configure(Config config)
        {
            this.config = config;
            client = new HttpClient();
        }

        public async void Execute(SystemMetrics systemInfo)
        {
            try
            {
                if (client != null)
                {
                    var response = await client.PostAsJsonAsync(
                        config?.Monitoring.ApiEndpoint,
                        new
                        {
                            cpu = systemInfo.CpuUsagePercentage,
                            ram_used = systemInfo.RamUsed,
                            diskUsed = systemInfo.DiskUsed
                        });
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("[API Publisher] API Response \n " + result);

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in sending API response: "+ ex);
            }
            
               
            
        }
    }
}
