using DbLib; // Namespace der zu testenden Klasse
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject1
{
    [TestClass]
    public class MySqlAccessTests
    {
        [TestMethod]
        public void Test_OpenConnection_Success()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<MySqlAccess>>(); // Verwende Moq f�r den Logger
            var mySqlAccess = new MySqlAccess("testdb", "localhost", "root", "password", loggerMock.Object);

            // Act
            var result = mySqlAccess.openConnection();

            // Assert
            Assert.AreEqual(errorValues.Success, result); // Pr�ft, ob der R�ckgabewert "Success" ist
        }
    }
}