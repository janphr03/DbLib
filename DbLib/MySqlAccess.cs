﻿using System;
using System.Data;
using System.Diagnostics;
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
        
        public errorValues flagStatus = 0;  // Verbindungsstatus


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
                flagStatus = errorValues.Success;  
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
        public errorValues openConnection()
        {
            errorValues returnVal = errorValues.Success;
            try
            {
                // Prüft im Vorfeld, ob die Verbindung bereits besteht
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    returnVal = errorValues.Success;
                }

                // Prüfe, ob ein Verbindungsstring überhaupt vorhanden ist
                if (string.IsNullOrEmpty(connection.ConnectionString))
                {
                    returnVal = errorValues.ConnectionQueryError; // Fehlercode -1 für ungültige Verbindungszeichenfolge
                }

                // Prüfen, ob der Server erreichbar ist durch Anpingen
                using (Ping ping = new Ping()){

                    PingReply reply = ping.Send(server);

                    if (reply.Status != IPStatus.Success)
                        {
                            returnVal = errorValues.ServerConnectionFailed; // Fehlercode -2 für nicht erreichbaren Server
                        }
                }

                // Öffne die Verbindung, wenn sie noch nicht offen ist
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                returnVal = errorValues.Success;

            }
            // Allgemeiner/ unbekannter Fehler
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
                
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
        public errorValues closeConnection()
        {
             errorValues returnVal = errorValues.Success;
            try
            {

                // Es ist keine gültige Verbindung vorhanden welche man schließen könnte
                if (connection == null)
                {
                    returnVal = errorValues.ConnectionInvalid; // Fehlercode -1 für ungültige Verbindung
                }

                // Überprüfen, ob die Verbindung bereits geschlossen ist
                else if (connection.State == System.Data.ConnectionState.Closed)
                {
                    returnVal = errorValues.ConnectionAlreadClosed;
                }


                // Schließe die Verbindung, wenn sie offen ist
                else
                {
                    connection.Close();
                }
                
            }
            // Allgemeiner/ unbekannter Fehler
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError; 
            }
            finally{ 
            }

            return returnVal;
        }


//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public errorValues executeQuery(string query)
        {

            errorValues returnVal = errorValues.Success;  // Erstellen eines leeren DataTable
            try
            {

                DataTable dt = new DataTable();
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

                if (dt.Rows.Count == 0)
                {
                    returnVal = errorValues.NoData;
                }

            }

            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
            }

            finally
            {
            }
            return returnVal;
        }


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
        public errorValues select(string column, string tableName, string whereCondition = "", string orderBy = "")
        {
            errorValues returnVal = errorValues.Success;    
            string querySelect = "";
            try
            {
                DataTable dt = new DataTable();
                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    openConnection();
                }

                // SQL-Abfrage-String zusammensetzen
                if(string.IsNullOrEmpty(column) || string.IsNullOrEmpty(tableName))
                {
                    returnVal = errorValues.emptyInputParameters;
                }
                else
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

                // MySqlCommand erstellen und Abfrage ausführen nur wenn es eine Query gibt
                if (!string.IsNullOrEmpty(querySelect))
                {
                    using (MySqlCommand cmd = new MySqlCommand(querySelect, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Lade die Spaltenstruktur des DataReaders in den DataTable
                            dt.Load(reader);
                        }
                    }
                }
                // DataTable hat keinen Inhalt
                if (dt.Rows.Count == 0)
                {
                    returnVal = errorValues.NoData;
                }


                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        Console.Write($"{row[col]} | ");  // Gibt den Wert der aktuellen Zelle aus
                    }
                    Console.WriteLine();  // Zeilenumbruch nach jeder Zeile
                }

            }
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
            }

            finally
            {
            }
            return returnVal;
        }

        public errorValues select(string query)
        {

            errorValues returnVal = errorValues.Success;  // Erstellen eines leeren DataTable
            try
            {

                DataTable dt = new DataTable();
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

                if (dt.Rows.Count == 0)
                {
                    returnVal = errorValues.NoData;
                }


                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        Console.Write($"{row[col]} | ");  // Gibt den Wert der aktuellen Zelle aus
                    }
                    Console.WriteLine();  // Zeilenumbruch nach jeder Zeile
                }


            }

            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
            }

            finally
            {
            }
            return returnVal;
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
        public errorValues update(string tableName, string set, string whereCondition = "", string join = "")
        {
             errorValues returnVal = errorValues.Success;
            string queryUpdate = "";
            try
            {
                // Sicherstellen, dass der tableName und das Set-Statement nicht leer sind (da Pflicht)
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(set))
                {
                    returnVal = errorValues.emptyInputParameters;
                }

                // Grundlegende MySql-Abfrage erstellen sofern tableName und set string-Werte enthalten
                else
                {
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

                if (!string.IsNullOrEmpty(queryUpdate)) { 

                // MySql-Befehl ausführen wenn die Query nicht leer ist
                using (MySqlCommand cmd = new MySqlCommand(queryUpdate, connection))
                {
                    int affectedRows = cmd.ExecuteNonQuery(); // Führt den Befehl aus und gibt die Anzahl der betroffenen Zeilen zurück
                    returnVal = affectedRows > 0 ? errorValues.Success : errorValues.NoData; // Setzt den Enum basierend auf dem Ergebnis
                }
            }
            }
            // Es ist ein anderer Fehler aufgetreten
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;  
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
        public errorValues insert(string tableName, string values)
        {
            // Speichert die Fehlermeldungen in Form von enum Werten
            errorValues returnVal = errorValues.Success;
            try
            {
                string queryInsert = "";
                // Sicherstellen, dass der tableName und values nicht leer sind
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(values))
                {
                    returnVal = errorValues.emptyInputParameters;
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

                if (!string.IsNullOrEmpty(queryInsert))
                {
                    // MySql-Befehl ausführen, wenn Query nicht leer ist
                    using (MySqlCommand cmd = new MySqlCommand(queryInsert, connection))
                    {
                        int affectedRows = cmd.ExecuteNonQuery(); // Führt den Befehl aus und speichert die Anzahl der betroffenen Zeilen
                        returnVal = affectedRows < 0 ? errorValues.Success : errorValues.NoData; //
                    }
                }
            }
            // Es ist ein anderer Fehler aufgetreten
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
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
        public errorValues delete(string tableName, string whereCondition, string limit)
        {
            errorValues returnVal = errorValues.Success;
            try
            {
                string queryDelete = "";
                // Sicherstellen, dass der tableName und die whereCondition nicht leer sind
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(whereCondition))
                {
                    returnVal = errorValues.emptyInputParameters;
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
                if (string.IsNullOrEmpty(queryDelete))
                {
                    using (MySqlCommand cmd = new MySqlCommand(queryDelete, connection))
                    {
                        int affectedRows = cmd.ExecuteNonQuery(); // Führt den Befehl aus und speichert die Anzahl der betroffenen Zeilen
                        returnVal = affectedRows > 0 ? errorValues.Success : errorValues.NoData;
                    }
                }
            }
            // allgemeiner Fehler
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
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