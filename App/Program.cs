using System.Data;
using System.Diagnostics;
    public enum errorValues
    {
        
        ungueltigeVerbindungszeichenfolge = -1,  // 0
        serverNichtErreichbar = -2,  // 1
        keineVerbindungsinformationenErhalten = -3,  // 2
        Wednesday = 0b_0000_0100,  // 4
        Thursday = 0b_0000_1000,  // 8
        Friday = 0b_0001_0000,  // 16
        Saturday = 0b_0010_0000,  // 32
        Sunday = 0b_0100_0000,  // 64
        Weekend = Saturday | Sunday
}

namespace DbLib
{

    public class Program
    {

        public static void Main(string[] args)
        {

         
            
            // Die Verbindung wird zu einer beliebigen DB hergestellt (derzeit nur MySql)
            IConnector connector = new MySqlAccess("testprotocol", "localhost", "root", "password");
            // connector.select("*", "employees", "", "");

            //connector.update("employees", "last_name = 'Herrmann'", "employee_id = 1", "");
            //connector.insert("employees", "7, 'Mo', 'Zo'");
            //connector.delete("employees", "last_name = 'Zo'", "1"

            //DataTable resultTable = connector.select("*", "testcase, tester", ".TesterID = tester.TesterID", "");






            Guid myuuid = Guid.NewGuid(); 
            String uuid = myuuid.ToString();

            DataTable resultTable = connector.executeQuery(" Select * FROM testprotocol JOIN tester ON testprotocol.TesterID = tester.TesterID WHERE tester.TesterID = 1");

            foreach (DataRow row in resultTable.Rows)
            {
            foreach (DataColumn column in resultTable.Columns)
                {
                    Console.Write($"{row[column]} | ");  // Gibt den Wert der aktuellen Zelle aus
                }
                Console.WriteLine();  // Zeilenumbruch nach jeder Zeile
            }

        }
    }   
}

