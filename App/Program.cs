using DbLib;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        string server = "localhost";
        string database = "testprotocol";
        string user = "root";
        string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD"); // Passwort ist als Systemvariable hinterlegt

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

        // Erstelle die MySQL-Verbindung
        string connectionString = $"Server={server};Database={database};Uid={user};Pwd={password};";
        // var connection = new MySqlConnection(connectionString);


        try
        {
            // MySqlAccess-Instanz mit der Verbindung erstellen
            IConnector mySqlAccess = new MySqlAccess("localhost", "testprotocol", "root", "password", logger);

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

        // Beende das Logging
        Log.CloseAndFlush();
    }
}

