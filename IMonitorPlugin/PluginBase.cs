namespace IMonitorPluginBase;

public class Config
{
    public required Monitoring Monitoring { get; set; }
    public required Logging Logging { get; set; }
}

public class Monitoring
{
    public int IntervalSeconds { get; set; }
    public required string ApiEndpoint { get; set; }
    public required string FilePath { get; set; }
}

public class Logging
{
    public required string LogLevel { get; set; }
}
public class SystemMetrics
{
    public double CpuUsagePercentage { get; set; }
    public double RamUsed { get; set; }
    public double RamUsedTotal { get; set; }
    public double DiskUsed { get; set; }
    public double DiskUsedTotal { get; set; }
    public DateTime Timestamp { get; set; }
    public override string ToString()
    {
        return $"\n{Timestamp}, " +
            $"CPU Usage: {CpuUsagePercentage}%, " +
            $"RAM Used: {RamUsed}/{RamUsedTotal} MB, " +
            $"Disk Used: {DiskUsed}/{DiskUsedTotal} MB";
    }
}
public interface IMonitorPlugin
{
    string Name { get; }
    string Description { get; }
    void Configure(Config config);
    void Execute(SystemMetrics systemInfo);
}
