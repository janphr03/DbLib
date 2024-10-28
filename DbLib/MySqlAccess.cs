using System;
using System.Data;
using System.Net.NetworkInformation;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;

namespace DbLib
{
    public class MySqlAccess : IConnector
    {

        private string database;
        private string server;
        private string uid;
        private string password;

        private MySql.Data.MySqlClient.MySqlConnection connection;
        
        public int flagStatus = 0;  // Verbindungsstatus


        /// <summary>
        /// Der Konstruktor öffnet die Connection und speichert den return-Wert im flagStatus, damit man sehen kann ob die Verbindung aufgebaut wurde
        /// </summary>
        /// 
        /// <param name="database"></param>
        /// <param name="server"></param>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// 
        /// flagStatus = 0 -> Die Verbindung konnte hergestellt werden
        /// flagStatus != 0 -> Beim Herstellen der Verbindung muss ein Fehler aufgetreten sein und das Objekt wurde fehlerhaft instanziiert
        public MySqlAccess(string database, string server, string uid, string password)
        {
            // Überprüfen ob die Verbindungsparameter enthalten sind
            // flagStatus != 0 für ungültige Verbindung
            if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(password))
            {
                flagStatus = -1;  
                return;
            }

            // Verbindung mit den Parametern herstellen
            this.database = database;
            this.server = server;
            this.uid = uid;
            this.password = password;
            connection = new MySql.Data.MySqlClient.MySqlConnection($"Server={server};Database={database};Uid={uid};Pwd={password};");

            // Verbindung öffnen 
            flagStatus = openConnection();

        }

        // Versucht die Verbindung zur Datenbank über den MySql Connector herzustellen.
        // Falls die Verbindung schon besteht, wird der Prozess frühzeitig beendet.

        /// <summary> 
        /// Versucht die Verbindung zur Datenbank über den MySql Connector herzustellen.
        /// Falls die Verbindung schon besteht, wird der Prozess frühzeitig beendet.
        /// </summary>
        /// 
        /// <returns>
        ///  0 = offen
        /// -1 = keine Verbindungsinformationen erhalten
        /// -2 = Server nicht erreichbar
        /// -3 = anderer Fehler
        /// </returns>
        public int openConnection()
        {
            int returnVal = 0;
            try
            {
                // Prüft im Vorfeld, ob die Verbindung bereits besteht
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    returnVal = 0;
                }

                // Prüfe, ob ein Verbindungsstring überhaupt vorhanden ist
                if (string.IsNullOrEmpty(connection.ConnectionString))
                {
                    returnVal = -1; // Fehlercode -1 für ungültige Verbindungszeichenfolge
                }

                // Prüfen, ob der Server erreichbar ist durch Anpingen
                using (Ping ping = new Ping()){

                    PingReply reply = ping.Send(server);

                    if (reply.Status != IPStatus.Success)
                        {
                            returnVal = -2; // Fehlercode -2 für nicht erreichbaren Server
                        }
                }

                // Öffne die Verbindung, wenn sie noch nicht offen ist
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                returnVal = 0;

            }
            // Allgemeiner Fehler
            catch (Exception e)
            {
                returnVal = -3;
                
            }
            finally { 
            }
            return returnVal;

            // Fehler falls die Verbindung nicht geöffnet werden konnte
        }



        /// <summary>
        /// Die Verbindung zu MySqlDb wird geschlossen
        /// </summary>
        /// 
        /// <returns>
        ///  0 = geschlossen
        /// -1 = keine Verbindungsinformationen erhalten
        /// -2 = Verbindung schon geschlossen
        /// -3 = anderer Fehler ist aufgetreten
        /// </returns>
        public int closeConnection()
        {
            int returnVal;
            try
            {

                // Es ist keine gültige Verbindung vorhanden welche man schließen könnte
                if (connection == null)
                {
                    returnVal = -1; // Fehlercode -1 für ungültige Verbindung
                }

                // Überprüfen, ob die Verbindung bereits geschlossen ist
                else if (connection.State == System.Data.ConnectionState.Closed)
                {
                    returnVal = -2;
                }


                // Schließe die Verbindung, wenn sie offen ist
                else
                {
                    connection.Close();
                }
                returnVal = 0; // Verbindung wurde erfolgreich geschlossen

            }
            // Allgemeiner Fehler
            catch (Exception e)
            {
                returnVal = -3; 
            }
            finally{ 
            }

            return returnVal;
        }


        // ---------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// erhält festgelegte Parameter beim Methodenaufruf, welche zu einem MySql Befehl zusammengeführt werden.
        /// Aus diesen Befehlen wird dann ein MySql Select Aufruf erstellt.
        /// </summary>
        /// 
        /// <param name="column">Die Spalte</param>
        /// <param name="tableName"></param>
        /// <param name="whereCondition"></param>
        /// <param name="orderBy"></param>
        /// 
        /// <returns>
        /// Es wird der DataTable als Objekt zurückgegeben
        /// </returns>
        public DataTable select(string column, string tableName, string whereCondition = "", string orderBy = "")
        {

            DataTable dt = null;  // Erstellen eines leeren DataTable
            string querySelect = "";
            try
            {
                dt = new DataTable();
                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    openConnection();
                }

                // SQL-Abfrage-String zusammensetzen
                if (!string.IsNullOrEmpty(column) || !string.IsNullOrEmpty(tableName))
                {
                    querySelect = $" SELECT {column} FROM {tableName}";
                }

                // WHERE-Bedingung hinzufügen falls vorhanden
                if (!string.IsNullOrEmpty(whereCondition))
                {
                    querySelect += $" WHERE {whereCondition}";
                }

                // ORDER BY hinzufügen falls vorhanden
                if (!string.IsNullOrEmpty(orderBy))
                {
                    querySelect += $" ORDER BY {orderBy}";
                }

                // MySqlCommand erstellen und Abfrage ausführen
                using (MySqlCommand cmd = new MySqlCommand(querySelect, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Lade die Spaltenstruktur des DataReaders in den DataTable
                        dt.Load(reader);
                    }
                }                
            }

            catch (Exception e)
            {
            }

            finally
            {
            }
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable executeQuery(string query)
        {

            DataTable dt = null;  // Erstellen eines leeren DataTable
            try
            {
                dt = new DataTable();
                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    openConnection();
                }

               // MySqlCommand erstellen und Abfrage ausführen
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Lade die Spaltenstruktur des DataReaders in den DataTable
                        dt.Load(reader);
                    }
                }                
            }

            catch (Exception e)
            {
            }

            finally
            {
            }
            return dt;
        }

        /// <summary>
        /// Die Methode bekommt die Information für eine Sql UPDATE-Abfrage in Form mehrerer vordefinierter String Parameter.
        /// Die Aufgabe der Methode ist es, die Einzelnen Teile der Eingabe zu einem einzigen String zusammenzuführen
        /// </summary>
        /// 
        /// <param name="tableName"></param>
        /// <param name="set"></param>
        /// <param name="whereCondition"></param>
        /// <param name="join"></param>
        /// 
        /// <returns>
        ///  n = Anzahl betroffener Felder
        /// -1 = tableName oder set Wert leer
        /// -2 = anderer Fehler
        /// </returns>
        public int update(string tableName, string set, string whereCondition = "", string join = "")
        {
            int returnVal = 0;
            string queryUpdate = "";
            try
            {
                // Sicherstellen, dass der tableName und das Set-Statement nicht leer sind (da Pflicht)
                if (string.IsNullOrEmpty(tableName) ||string.IsNullOrEmpty(set))
                {
                    returnVal = -1;
                }

                // Grundlegende MySql-Abfrage erstellen sofern tableName und set string-Werte enthalten
                else{
                    queryUpdate = $" UPDATE {tableName}";

                    // SET-Teil der Abfrage hinzufügen
                    queryUpdate += $" SET {set}";
                }

                // JOIN hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(join))
                {
                    queryUpdate += $" {join}";
                }

                
                // WHERE-Bedingung hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(whereCondition))
                {
                    queryUpdate += $" WHERE {whereCondition}";
                }

                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    openConnection();
                }

                // MySql-Befehl ausführen
                using (MySqlCommand cmd = new MySqlCommand(queryUpdate, connection))
                {
                    returnVal = cmd.ExecuteNonQuery(); // Führt den Befehl aus und gibt die Anzahl der betroffenen Zeilen zurück
                }
            }
            // Es ist ein anderer Fehler aufgetreten
            catch (Exception e)
            {
                returnVal = -2;  
            }

            finally 
            { 
            }

            return returnVal;
        }


        /// <summary>
        /// Die Methode bekommt die Information für eine Sql INSERT-Abfrage in Form mehrerer vordefinierter String Parameter.
        /// Die Aufgabe der Methode ist es, die Einzelnen Teile der Eingabe zu einem einzigen String zusammenzuführen.
        /// </summary>
        /// 
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// 
        /// <returns>
        ///  n = Anzahl betroffener Felder
        /// -1 = tableName oder values leer
        /// -2 = anderer Fehler
        /// </returns>
        public int insert(string tableName, string values)
        {
            // Speichert die Fehlermeldungen in Form von int Werten
            int returnVal = 0;
            try
            {
                string queryInsert = "";
                // Sicherstellen, dass der tableName und values nicht leer sind
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(values))
                {
                    returnVal = -1;
                }
                // Grundlegende MySql-Abfrage erstellen, sofern table name und values nicht leer sind
                else
                {
                    queryInsert = $" INSERT INTO {tableName}"; 
                }


                // WHERE-Bedingung hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(values))
                {
                    queryInsert += $" VALUES({values});";
                }


                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    openConnection();
                }

                // nur ausführen wenn Verbindung offen
                // MySql-Befehl ausführen
                using (MySqlCommand cmd = new MySqlCommand(queryInsert, connection))
                {
                    returnVal = cmd.ExecuteNonQuery(); // Führt den Befehl aus und speichert die Anzahl der betroffenen Zeilen
                }
            }
            // Es ist ein anderer Fehler aufgetreten
            catch (Exception e)
            {
                returnVal = -2;
            }

            finally
            {
            }

            return returnVal;
        }


        /// <summary>
        /// Die Methode bekommt die Information für eine Sql DELETE-Abfrage in Form mehrerer vordefinierter String Parameter. 
        /// Die Aufgabe der Methode ist es, die Einzelnen Teile der Eingabe zu einem einzigen String zusammenzuführen.
        /// </summary>
        /// 
        /// <param name="tableName"></param>
        /// <param name="whereCondition"></param>
        /// 
        /// <returns>
        ///  n = Anzahl betroffener Felder
        /// -1 = tableName oder whereCondition leer
        /// -2 = anderer Fehler
        /// </returns>
        public int delete(string tableName, string whereCondition, string limit)
        {
            int returnVal = 0;
            try
            {
                string queryDelete = "";
                // Sicherstellen, dass der tableName und die whereCondition nicht leer sind
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(whereCondition))
                {
                    returnVal = -1;
                }
                // Grundlegende MySql-Abfrage erstellen, sofern table name und values nicht leer sind
                else
                {
                    queryDelete = $" DELETE FROM {tableName} WHERE {whereCondition}";
                }

                // WHERE-Bedingung hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(limit))
                {
                    queryDelete += $" LIMIT {limit};";
                }

                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    openConnection();
                }

                // MySql-Befehl ausführen
                using (MySqlCommand cmd = new MySqlCommand(queryDelete, connection))
                {
                    returnVal = cmd.ExecuteNonQuery(); // Führt den Befehl aus und speichert die Anzahl der betroffenen Zeilen
                }

            }
            // allgemeiner Fehler
            catch (Exception e)
            {
                returnVal = -2;
            }
            finally
            {
            }

            return returnVal;
        }


        ~MySqlAccess()
        {
            closeConnection();
        }
    }
}
