using CourseService.Application.Abstraction.Services;

namespace CourseService.API.Services
{
    public class FileLoggerService : ILoggerService
    {
        private readonly string _logFilePath;

        public FileLoggerService(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void LogInformation(string message)
        {
            LogToFile($"INFO: {message}");
        }

        public void LogWarning(string message)
        {
            LogToFile($"WARNING: {message}");
        }

        public void LogError(string message, Exception ex = null)
        {
            LogToFile($"ERROR: {message}\nException: {ex?.ToString()}");
        }

        private void LogToFile(string logEntry)
        {
            // Dosyaya log ekleme işlemi
            File.AppendAllText(_logFilePath, $"{DateTime.Now} - {logEntry}\n");
        }
    }
}
