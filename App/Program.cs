using System.Data;
using System.Diagnostics;

namespace DbLib
{

    public class Program
    {

        public static void Main(string[] args)
        {

         
            
            // Die Verbindung wird zu einer beliebigen DB hergestellt (derzeit nur MySql)
            IConnector connector = new MySqlAccess("testprotocol", "localhost", "root", "password");

            int res = (int)connector.openConnection();
            Console.WriteLine(res);
            // connector.select("*", "employees", "", "");

            //connector.update("employees", "last_name = 'Herrmann'", "employee_id = 1", "");
            //connector.insert("employees", "7, 'Mo', 'Zo'");
            //connector.delete("employees", "last_name = 'Zo'", "1"

            //DataTable resultTable = connector.select("*", "testcase, tester", ".TesterID = tester.TesterID", "");






            Guid myuuid = Guid.NewGuid(); 
            String uuid = myuuid.ToString();


        }
    }   
}

