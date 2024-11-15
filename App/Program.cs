using App;

namespace DbLib
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string logFilePath = "C:\\Users\\janph\\Source\\Repos\\App\\App\\Log.txt";

            // Sicherstellen, dass die Datei existiert
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose(); // Datei erstellen und freigeben
            }

            ILogger logger = new FileLogger(logFilePath);

            try
            {
                // Die Verbindung wird zu einer beliebigen DB hergestellt (derzeit nur MySql)
                IConnector connector = new MySqlAccess("testprotocol", "localhost", "root", "cnxx0383");

                logger.Info("Datenbankverbindung erfolgreich hergestellt.");

                // Beispiel einer SELECT-Abfrage
                var result = connector.select("*", "tester", "", "");
                Console.WriteLine(result);
                logger.Info("SELECT-Abfrage erfolgreich ausgeführt.");
            }
            catch (Exception ex)
            {
                logger.Error("Fehler in der Main-Methode.", ex);
                Console.WriteLine("Ein Fehler ist aufgetreten. Details im Logfile.");
            }
        }
    }
}