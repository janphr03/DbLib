using Moq;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;
using Mysqlx.Expect;

namespace DbLib.UnitTests
{

    // Testaufbau (AAA)

    // Act
    // Assert
    // Arrange


    public class MySqlAccessTests
    {

        private readonly string _password;

        public MySqlAccessTests()
        {
            // Passwort aus Umgebungsvariable holen
            _password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        //                            Konstruktor 
        //-------------------------------------------------------------------------------------------------------------------------------------------------------



        // positiv Tests

        [Fact]
        public void Constructor_InitializesCorrectly_WhenParametersAreValid()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MySqlAccess>>();

            string database = "testprotocol";
            string server = "localhost";
            string uid = "root";
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            string connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
            var connection = new MySqlConnection(connectionString);

            // Act
            var mySqlAccess = new MySqlAccess(connection, mockLogger.Object);
            // Assert
            Assert.NotNull(mySqlAccess);
            Assert.Equal(errorValues.Success, mySqlAccess.flagStatus); // Verbindung sollte erfolgreich sein
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

            string connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
            var connection = new MySqlConnection(connectionString);

            // Act
            // Erstellen einer Instanz von MySqlAccess mit gültigen Parametern
            var mySqlAccess = new MySqlAccess(connection, mockLogger.Object);

            // Assert
            // Überprüfen, dass der Flag-Status auf Erfolg gesetzt wurde, da die Verbindung erfolgreich war
            Assert.Equal(errorValues.Success, mySqlAccess.flagStatus);
        }



        // negativ Tests

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

            string connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
            var connection = new MySqlConnection(connectionString);

            // Act & Assert
            // Der Konstruktor-Aufruf ohne Logger soll eine ArgumentNullException auslösen
            Assert.Throws<ArgumentNullException>(() => new MySqlAccess(connection, null));
        }


        [Theory]
        [InlineData(null)] // Verbindung ist null
        public void Constructor_SetsFlagStatusToError_WhenConnectionIsInvalid(MySqlConnection connection)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MySqlAccess>>();

            // Act
            var mySqlAccess = new MySqlAccess(connection, mockLogger.Object);

            // Assert
            Assert.Equal(errorValues.EmptyInputParameters, mySqlAccess.flagStatus); // Verbindung sollte als ungültig erkannt werden
        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        //                              openConnection
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        [Fact]
        public void OpenConnection_ReturnsSuccess_WhenConnectionIsOpenedSuccessfully()
        {
            // Arrange
            string database = "testprotocol"; // Name der Datenbank
            string server = "localhost"; // Hostname oder IP-Adresse des Servers
            string uid = "root"; // Benutzername für die Datenbankverbindung
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "defaultpassword";

            string connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
            using var connection = new MySqlConnection(connectionString);
            var mockLogger = new Mock<ILogger<MySqlAccess>>();

            var mySqlAccess = new MySqlAccess(connection, mockLogger.Object);

            // Act
            var result = mySqlAccess.openConnection();

            // Assert
            Assert.Equal(errorValues.Success, result); // Verbindung sollte erfolgreich geöffnet werden
        }






        // positiv Tests


        // negativ Tests


        // Logging Tests


        // Connection State Prüfen


        // exception Handling Tests


        // Edge Cases (allg Exception)


    }
}

