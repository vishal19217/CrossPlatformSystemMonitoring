using IMonitorPluginBase;
using System.Collections.Concurrent;
using System.Reflection;

namespace Logger;

public class FileLogger : IMonitorPlugin
{
    private Config? config;
    private static ConcurrentQueue<string> _textToWrite = new ConcurrentQueue<string>();
    private Mutex _mutex = new Mutex();
    public string Name
    {
        get { return "File Logger Plugin"; }
    }

    public string Description
    {
        get { return "Logs system metrics to a file."; }
    }

    public void Execute(SystemMetrics systemInfo)
    {
        try
        {
            WriteToFile(systemInfo);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FileLogger] Error writing to log file: {ex.Message}");
        }
    }

    public void Configure(Config config)
    {
        this.config = config;


    }
    private async void WriteToFile(SystemMetrics systemInfo)
    {
        try
        {
            _mutex.WaitOne();
            {
                _textToWrite.Enqueue(systemInfo.ToString());
                string dllPath = Assembly.GetExecutingAssembly().Location;
                string dllDirectory = Path.GetDirectoryName(dllPath)!;


                string logFilePath = Path.Combine(dllDirectory, config.Monitoring.FilePath);

                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    while (_textToWrite.TryDequeue(out string logEntry))
                    {
                        await writer.WriteLineAsync(logEntry);

                        Console.WriteLine("[FileLogger]" + "Successfully written in file....");
                    }
                    writer.Flush();
                }


            }
            _mutex.ReleaseMutex();
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error in writing to file: ", ex);
        }
        

    }
}
