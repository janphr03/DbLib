using DbLib;
using Microsoft.Extensions.Logging;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        string server = Environment.GetEnvironmentVariable("MYSQL_SERVER") ?? "localhost";
        string database = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "testprotocol";
        string user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
        string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";

        Console.WriteLine($"MYSQL_SERVER: {server}");
        Console.WriteLine($"MYSQL_DATABASE: {database}");
        Console.WriteLine($"MYSQL_USER: {user}");
        Console.WriteLine($"MYSQL_PASSWORD: {password}");

        // Überprüfen, ob die Variablen korrekt geladen wurden
        if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database) ||
            string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Umgebungsvariablen für die MySQL-Verbindung sind nicht vollständig gesetzt.");
            return;
        }

        // Konfiguriere Serilog, um in eine Datei zu loggen
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(@"C:\logs\logfile.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Erstelle eine LoggerFactory und füge Serilog als Logger hinzu
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

            // Beispielaufruf für eine Methode
            var status = mySqlAccess.select("*", "tester", "", "");
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

        // Beende das Logging (optional, wird normalerweise beim Programmende automatisch aufgerufen)
        Log.CloseAndFlush();
    }
}
