using System;
using System.IO;

namespace App
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        // Optional: Werte für Benutzername und IP festlegen, falls diese statisch sein sollen
        private readonly string _ipAddress = "-";
        private readonly string _username = "-";

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        private void Log(string level, string message, int statusCode = 200, int dataSize = 0, string referer = "-", string userAgent = "-")
        {
            // Formatierung des Datums im Common Log Format
            string logMessage = $"{_ipAddress} {_username} [{DateTime.Now:dd/MMM/yyyy:HH:mm:ss zzz}] \"{level}: {message}\" {statusCode} {dataSize} \"{referer}\" \"{userAgent}\"";

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
            Log("ERROR", errorMessage, 500);  // Fehlercode 500 für allgemeine Fehler
        }
    }
}

