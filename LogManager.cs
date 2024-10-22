using Serilog;

namespace Reconsile
{
    public static class LogManager
{
    public static ILogger Logger { get; private set; }

    static LogManager()
    {
        string logFileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";

        Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File($"logs/{logFileName}", 
                rollingInterval: RollingInterval.Day, 
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}")
            .CreateLogger();
    }
}
}
