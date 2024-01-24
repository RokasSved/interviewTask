namespace FleetManager;

internal class Log
{
    private readonly string _logFile;

    public Log()
    {
        _logFile = "C:\\Logs\\InterviewTask.log";
    }

    public void Info(string message)
    {
        File.AppendAllText(_logFile, $"{DateTime.UtcNow} - INFO - {message}");
    }

    public void Error(string message)
    {
        File.WriteAllText(_logFile, $"{DateTime.UtcNow} - ERROR - {message}");
    }
}