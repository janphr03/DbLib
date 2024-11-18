using DbLib;
using Microsoft.Extensions.Logging;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        // Konfiguriere Serilog, um in eine Datei zu loggen
        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
        .WriteTo.File(@"C:\logs\logfile.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();        
        
        // Erstelle eine LoggerFactory und füge Serilog als Logger hinzu
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(); // Serilog als Logging-Provider hinzufügen
        });

        // Logger für MySqlAccess erstellen
        ILogger<MySqlAccess> logger = loggerFactory.CreateLogger<MySqlAccess>();

        // MySqlAccess-Instanz mit dem Logger erstellen
        var mySqlAccess = new MySqlAccess("testprotocol", "localhost", "root", "password", logger);

        // Beispielaufruf für eine Methode
        mySqlAccess.select("*", "tester");

        // Beende das Logging (optional, wird normalerweise beim Programmende automatisch aufgerufen)
        Log.CloseAndFlush();
    }
}
