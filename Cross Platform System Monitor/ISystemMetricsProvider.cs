using IMonitorPluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross_Platform_System_Monitor
{
    public interface ISystemMetricsProvider
    {
        public SystemMetrics GetSystemMetrics();
    }
}
