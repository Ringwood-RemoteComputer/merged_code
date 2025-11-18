using System;
using System.IO;

namespace Ring.Services
{
    /// <summary>
    /// Implementation of ILogger for debugging and monitoring
    /// </summary>
    public class Logger : ILogger
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the Logger
        /// </summary>
        /// <param name="logFilePath">Path to the log file (optional)</param>
        public Logger(string logFilePath = null)
        {
            _logFilePath = logFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "application.log");
            
            // Ensure log directory exists
            var logDirectory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="message">The message to log</param>
        public void LogInfo(string message)
        {
            LogMessage("INFO", message);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">The message to log</param>
        public void LogWarning(string message)
        {
            LogMessage("WARN", message);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception that occurred (optional)</param>
        public void LogError(string message, Exception exception = null)
        {
            var fullMessage = message;
            if (exception != null)
            {
                fullMessage += $"\nException: {exception.Message}\nStack Trace: {exception.StackTrace}";
            }
            LogMessage("ERROR", fullMessage);
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">The message to log</param>
        public void LogDebug(string message)
        {
            LogMessage("DEBUG", message);
        }

        /// <summary>
        /// Logs a message with the specified level
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        private void LogMessage(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logEntry = $"[{timestamp}] [{level}] {message}";

            // Write to console (for debugging)
            System.Diagnostics.Debug.WriteLine(logEntry);

            // Write to file
            lock (_lockObject)
            {
                try
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    // If file logging fails, at least log to console
                    System.Diagnostics.Debug.WriteLine($"Failed to write to log file: {ex.Message}");
                }
            }
        }
    }
}
