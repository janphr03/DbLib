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

            // connector.select("*", "employees", "", "");


            //Console.WriteLine(connector.insert("", ""));
            //connector.insert("employees", "7, 'Mo', 'Zo'");
            //connector.delete("employees", "last_name = 'Zo'", "1"

            //DataTable resultTable = connector.select("*", "testcase, tester", ".TesterID = tester.TesterID", "");
            
            Console.WriteLine (connector.select("Select * From Tester;"));



        }
    }   
}

