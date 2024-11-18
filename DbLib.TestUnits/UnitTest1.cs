using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;

namespace DbLib.UnitTests
{
    public class MySqlAccessTests
    {

        [Fact]
        public void Constructor_InitializesCorrectly_WhenParametersAreValid()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MySqlAccess>>();
            string database = "testprotocol";
            string server = "localhost";
            string uid = "root";
            string password = "password";

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
            string password = "password";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MySqlAccess(database, server, uid, password, null));
        }


    }


}
