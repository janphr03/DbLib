using Moq;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace DbLib.UnitTests
{
    public class MySqlAccessTests
    {


        private readonly string _password;

        public MySqlAccessTests()
        {
            // Passwort zentral abrufen und speichern
            _password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD")
                        ?? throw new InvalidOperationException("MYSQL_PASSWORD not set.");
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        //                            Konstruktor 
        //-------------------------------------------------------------------------------------------------------------------------------------------------------



        [Fact]
        public void Constructor_InitializesCorrectly_WhenParametersAreValid()
        {
            // Arrange
            // Erstelle einen Mock für den Logger, um die Protokollierungslogik zu testen
            var mockLogger = new Mock<ILogger<MySqlAccess>>();

            // Definiere gültige Verbindungsparameter
            string database = "testprotocol"; // Name der Datenbank
            string server = "localhost";     // Hostname oder IP-Adresse des Servers
            string uid = "root";             // Benutzername für die Datenbankverbindung
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");     

            // Act
            // Erstelle eine Instanz von MySqlAccess mit den definierten Parametern
            var mySqlAccess = new MySqlAccess(database, server, uid, password, mockLogger.Object);

            // Assert
            // Überprüfe, dass die Instanz korrekt initialisiert wurde
            Assert.NotNull(mySqlAccess); // Sicherstellen, dass die Instanz nicht null ist
            Assert.Equal(errorValues.Success, mySqlAccess.flagStatus);
            // Überprüfen, ob der Status auf Erfolg gesetzt wurde
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            // Arrange
            // Vorbereiten der Eingabewerte für den Test
            string database = "testDB"; // Name der Datenbank
            string server = "localhost"; // Hostname oder IP-Adresse des Servers
            string uid = "user"; // Benutzername für die Datenbankverbindung
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            // Passwort wird aus der Umgebungsvariable geladen

            // Act & Assert
            // Der Konstruktor-Aufruf ohne Logger soll eine ArgumentNullException auslösen
            Assert.Throws<ArgumentNullException>(() => new MySqlAccess(database, server, uid, password, null));
        }


        [Theory]
        [InlineData(null, "localhost", "root", "cnxx0383")] // Kein Datenbankname angegeben
        [InlineData("testDB", null, "root", "cnxx0383")]    // Kein Server angegeben
        [InlineData("testDB", "localhost", null, "cnxx0383")] // Kein Benutzername angegeben
        [InlineData("testDB", "localhost", "root", null)]     // Kein Passwort angegeben
        public void Constructor_SetsFlagStatusToError_WhenParametersAreInvalid(string database, string server, string uid, string password)
        {
            // Arrange
            // Erstellen eines Mock-Loggers für die Protokollierung
            var mockLogger = new Mock<ILogger<MySqlAccess>>();

            // Act
            // Konstruktor aufrufen mit den gegebenen Testdaten (eine oder mehrere Eingaben sind ungültig)
            var mySqlAccess = new MySqlAccess(database, server, uid, password, mockLogger.Object);

            // Assert
            // Überprüfen, dass der Konstruktor den Status auf Fehler setzt, wenn Eingaben ungültig sind
            Assert.Equal(errorValues.EmptyInputParameters, mySqlAccess.flagStatus);
        }


        [Fact]
        public void Constructor_SetsFlagStatusToSuccess_WhenConnectionIsSuccessful()
        {
            // Arrange
            // Erstellen eines Mock-Loggers, um die Logging-Funktionalität zu testen
            var mockLogger = new Mock<ILogger<MySqlAccess>>();

            // Definieren der Verbindungsparameter für eine erfolgreiche Verbindung
            string database = "testprotocol"; // Name der Datenbank
            string server = "localhost";     // Hostname oder IP-Adresse des Datenbankservers
            string uid = "root";             // Benutzername für die Verbindung
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            // Passwort wird sicher aus der Umgebungsvariablen geladen

            // Act
            // Erstellen einer Instanz von MySqlAccess mit gültigen Parametern
            var mySqlAccess = new MySqlAccess(database, server, uid, password, mockLogger.Object);

            // Assert
            // Überprüfen, dass der Flag-Status auf Erfolg gesetzt wurde, da die Verbindung erfolgreich war
            Assert.Equal(errorValues.Success, mySqlAccess.flagStatus);
        }


        [Fact]
        public void Constructor_SetsFlagStatusToError_WhenParametersContainSpecialCharacters()
        {
            // Arrange
            // Erstellen eines Mock-Loggers, um die Logging-Funktionalität zu testen
            var mockLogger = new Mock<ILogger<MySqlAccess>>();

            // Definieren von Eingabeparametern mit Sonderzeichen
            string database = "test-protocol"; // Datenbankname mit einem Sonderzeichen
            string server = "localhost!@#";   // Servername mit ungültigen Sonderzeichen
            string uid = "root";              // Gültiger Benutzername
            string password = "pass$word!";   // Passwort mit Sonderzeichen

            // Act
            // Erstellen einer Instanz von MySqlAccess mit ungültigen Parametern
            var mySqlAccess = new MySqlAccess(database, server, uid, password, mockLogger.Object);

            // Assert
            // Überprüfen, dass der Flag-Status auf Fehler gesetzt wurde, weil die Eingabeparameter Sonderzeichen enthalten
            Assert.Equal(errorValues.ConnectionQueryError, mySqlAccess.flagStatus);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        //                             openConnection
        //-------------------------------------------------------------------------------------------------------------------------------------------------------







    }
}

