using System;
using System.Data;
using DbLib;
using MySql.Data.MySqlClient;

namespace DbLib
{

    public class Program
    {
        public static void Main(string[] args)
        {



            // Die Verbindung wird zu einer beliebigen DB hergestellt (derzeit nur MySql)
            IConnector connector = new MySqlAccess("mydb", "localhost", "root", "password");
            connector.update("employees", "last_name = 'fsfds'", "employee_id = 1", "");
            connector.insert("employees", "6, 'test', 'nachname'");


            DataTable resultTable = connector.select("*", "employees", "", "");






            foreach (DataRow row in resultTable.Rows)
            {
                foreach (DataColumn column in resultTable.Columns)
                {
                    Console.Write($"{row[column]} ");  // Gibt den Wert der aktuellen Zelle aus
                }
                Console.WriteLine();  // Zeilenumbruch nach jeder Zeile
            }

        }
    }   
}

