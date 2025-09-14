using IMonitorPluginBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace Cross_Platform_System_Monitor.Platform
{
    public class WindowSystemMetricsProvider : ISystemMetricsProvider
    {


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        private PerformanceCounter cpuUsageCounter, memUsageCounter;
        public WindowSystemMetricsProvider()
        {
            cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuUsageCounter.NextValue();

            memUsageCounter = new PerformanceCounter("Memory", "Available MBytes");
            memUsageCounter.NextValue();

        }
        
        public SystemMetrics GetSystemMetrics()
        {

            double usedRam = getTotalRAMInMB() - Math.Round(memUsageCounter.NextValue(), 2);
            Tuple<double, double> internalMemory = GetInternalMemory();
            double usedDisk = internalMemory.Item2 - internalMemory.Item1;
            SystemMetrics metrics = new SystemMetrics
            {

                Timestamp = DateTime.Now,
                CpuUsagePercentage = Math.Round(cpuUsageCounter.NextValue(), 2),
                RamUsed = Math.Round(usedRam, 2),
                RamUsedTotal = getTotalRAMInMB(),
                DiskUsed = Math.Round(usedDisk, 2),
                DiskUsedTotal = Math.Round(internalMemory.Item2, 2)

            };
            return metrics;
        }

        private static Tuple<double, double> GetInternalMemory()
        {
            try
            {

                double totalInternalMemory=0, freeInternalMemory=0;
                {
                    DriveInfo[] allDrives = DriveInfo.GetDrives();
                    foreach (DriveInfo d in allDrives)
                    {
                        if (d.IsReady == true && d.Name.Contains("C:"))
                        {
                            decimal kbMemory = (decimal)d.AvailableFreeSpace;
                            decimal totalSize = (decimal)d.TotalSize;
                            kbMemory = kbMemory / (1024 * 1024 );  
                            totalSize = totalSize / (1024 * 1024);
                            freeInternalMemory = Math.Round((double)kbMemory,2);
                            totalInternalMemory = Math.Round((double)totalSize,2);
                        }
                    }
                }
                return new Tuple<double, double>(freeInternalMemory, totalInternalMemory);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Geting Internal Memory: ", ex);
                return new Tuple<double, double>(0, 0);
            }
        }
        public static float getTotalRAMInMB()//returns in MBytes
        {
            ulong installedMemory = 0;
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                installedMemory = memStatus.ullTotalPhys;
            }
            return (installedMemory / 1000000);
        }
    }
}
