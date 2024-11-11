using System;
using System.IO;

namespace App
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        private void Log(string level, string message)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            File.AppendAllText(_filePath, logMessage + Environment.NewLine);
        }

        public void Trace(string message)
        {
            Log("TRACE", message);
        }

        public void Debug(string message)
        {
            Log("DEBUG", message);
        }

        public void Info(string message)
        {
            Log("INFO", message);
        }

        public void Error(string message, Exception ex = null)
        {
            string errorMessage = message;
            if (ex != null)
            {
                errorMessage += $"{Environment.NewLine}Exception: {ex}";
            }
            Log("ERROR", errorMessage);
        }
    }
}

