﻿using DbLib;
using Microsoft.Extensions.Logging;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        string server = Environment.GetEnvironmentVariable("MYSQL_SERVER") ?? "localhost";
        string database = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "testprotocol";
        string user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
        string ?password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

        // Überprüfen, ob die Variablen korrekt geladen wurden
        if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database) ||
            string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Umgebungsvariablen für die MySQL-Verbindung sind nicht vollständig gesetzt.");
            return;
        }

        // Konfiguriert Serilog, um in eine Datei zu loggen
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(@"C:\logs\logfile.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Erstellt eine LoggerFactory und fügt Serilog als Logger hinzu
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(); // Serilog als Logging-Provider hinzufügen
        });

        // Logger für MySqlAccess erstellen
        ILogger<MySqlAccess> logger = loggerFactory.CreateLogger<MySqlAccess>();

        try
        {
            // MySqlAccess-Instanz mit den aus Umgebungsvariablen geladenen Werten erstellen
            IConnector mySqlAccess = new MySqlAccess(database, server, user, password, logger);


            var status = mySqlAccess.select("*", "tester", "", "");

            // Status testen
            if (status == errorValues.Success)
            {
                Console.WriteLine("Daten erfolgreich abgerufen.");
            }
            else
            {
                Console.WriteLine($"Fehler beim Abrufen der Daten: {status}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Fehler bei der MySQL-Verbindung oder Abfrage: {Message}", ex.Message);
        }

        // Beende das Logging
        Log.CloseAndFlush();
    }
}
