using Moq;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;

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
            var mockLogger = new Mock<ILogger<MySqlAccess>>();
            string database = "testprotocol";
            string server = "localhost";
            string uid = "root";
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            // Act
            var mySqlAccess = new MySqlAccess(database, server, uid, password, mockLogger.Object);

            // Assert
            Assert.NotNull(mySqlAccess);
            Assert.Equal(errorValues.Success, mySqlAccess.flagStatus); // Verifiziere, dass die Verbindung erfolgreich hergestellt wurde
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            // Arrange
            string database = "testDB";
            string server = "localhost";
            string uid = "user";
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MySqlAccess(database, server, uid, password, null));
        }

        [Theory]
        [InlineData(null, "localhost", "root", "cnxx0383")]
        [InlineData("testDB", null, "root", "cnxx0383")]
        [InlineData("testDB", "localhost", null, "cnxx0383")]
        [InlineData("testDB", "localhost", "root", null)]
        public void Constructor_SetsFlagStatusToError_WhenParametersAreInvalid(string database, string server, string uid, string password)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MySqlAccess>>();

            // Act
            var mySqlAccess = new MySqlAccess(database, server, uid, password, mockLogger.Object);

            // Assert
            Assert.Equal(errorValues.EmptyInputParameters, mySqlAccess.flagStatus);
        }

        [Fact]
        public void Constructor_SetsFlagStatusToSuccess_WhenConnectionIsSuccessful()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MySqlAccess>>();
            string database = "testprotocol";
            string server = "localhost";
            string uid = "root";
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            // Act
            var mySqlAccess = new MySqlAccess(database, server, uid, password, mockLogger.Object);

            // Assert
            Assert.Equal(errorValues.Success, mySqlAccess.flagStatus);
        }

        [Fact]
        public void Constructor_SetsFlagStatusToError_WhenParametersContainSpecialCharacters()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MySqlAccess>>();
            string database = "test-protocol"; // Sonderzeichen im Datenbanknamen
            string server = "localhost!@#";   // Sonderzeichen im Servernamen
            string uid = "root";
            string password = "pass$word!";   // Sonderzeichen im Passwort

            // Act
            var mySqlAccess = new MySqlAccess(database, server, uid, password, mockLogger.Object);

            // Assert
            Assert.Equal(errorValues.ConnectionQueryError, mySqlAccess.flagStatus);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        //                             openConnection
        //-------------------------------------------------------------------------------------------------------------------------------------------------------








    }
}

