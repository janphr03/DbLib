using System;
using System.Data;
using System.Net.NetworkInformation;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;

namespace DbLib
{
    public class MySqlAccess : IConnector
    {

        private string database;
        private string server;
        private string uid;
        private string password;

        private  MySql.Data.MySqlClient.MySqlConnection connection;
        private readonly ILogger<MySqlAccess> logger;
        
        public errorValues flagStatus = errorValues.Success;  // Verbindungsstatus




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
        public MySqlAccess(string database, string server, string uid, string password, ILogger<MySqlAccess>logger)
        {
            // Überprüfen ob die Verbindungsparameter enthalten sind
            // flagStatus errorValues.emptyParameters
            if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(password))
            {


                flagStatus = errorValues.EmptyInputParameters;  
                return;
            }

            // Verbindung mit den Parametern herstellen
            this.database = database;
            this.server = server;
            this.uid = uid;
            this.password = password;
            connection = new MySql.Data.MySqlClient.MySqlConnection($"Server={server};Database={database};Uid={uid};Pwd={password};");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            // Verbindung öffnen 
            flagStatus = openConnection();

        }
        /// <summary>
        /// Erstellt eine mockConnection zu testzwecken
        /// </summary>
        /// <param name="mockConnection"></param>
        public void SetConnectionForTesting(MySqlConnection mockConnection)
        {
            connection = mockConnection;
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
            logger?.LogInformation($"Attempting to open connection to: {this.database}");

            try
            {
                // Verbindung öffnen, falls sie nicht bereits offen ist
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    logger?.LogInformation("Connection already open.");
                    return errorValues.Success;
                }

                // Versuche, die Verbindung zu öffnen
                connection.Open();
                logger?.LogInformation("Connection opened successfully.");
                return errorValues.Success;
            }
            catch (InvalidOperationException ex) // Verbindung ist in einem ungültigen Zustand
            {
                logger?.LogError($"Invalid connection state: {ex.Message}");
                return errorValues.ConnectionInvalid;
            }
             catch (MySqlException ex) when (ex.Number == 1045) // Authentifizierungsfehler
            {
                logger?.LogError($"Authentication failed: {ex.Message}");
                return errorValues.AuthenticationFailed;
            }
            catch (MySqlException ex) when (ex.Number == 1049) // Datenbank nicht gefunden
            {
                logger?.LogError($"Database not found: {ex.Message}");
                return errorValues.DatabaseNotFound;
            }
            catch (MySqlException ex) when (ex.Number == 0) // Server nicht erreichbar
            {
                logger?.LogError($"Cannot connect to server: {ex.Message}");
                return errorValues.ServerConnectionFailed;
            }
            catch (MySqlException ex) // Allgemeiner MySQL-Fehler
            {
                logger?.LogError($"MySQL error: {ex.Message}");
                return errorValues.ConnectionQueryError;
            }
            catch (Exception ex) // Allgemeine Fehlerbehandlung
            {
                logger?.LogError($"An unknown error occurred: {ex.Message}");
                return errorValues.UnknownError;
            }
            finally
            {
                logger?.LogInformation("openConnection method completed.");
            }
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
            logger?.LogInformation("Attempting to close the connection.");

            try
            {
                // Prüfe, ob die Verbindung existiert
                if (connection == null)
                {
                    throw new InvalidOperationException("Connection is null."); // Werfe eine Ausnahme für ungültige Verbindung
                }

                // Prüfe, ob die Verbindung bereits geschlossen ist
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    throw new InvalidOperationException("Connection is already closed."); // Werfe eine Ausnahme für bereits geschlossene Verbindung
                }

                // Schließe die Verbindung, wenn sie offen ist
                connection.Close();
                logger?.LogInformation("Connection closed successfully.");
                return errorValues.Success;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("null"))
            {
                logger?.LogWarning("Connection is invalid or null.");
                return errorValues.ConnectionInvalid; // Fehlercode für ungültige Verbindung
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("closed"))
            {
                logger?.LogInformation("Connection is already closed.");
                return errorValues.ConnectionAlreadyClosed; // Fehlercode für bereits geschlossene Verbindung
            }
            catch (MySqlException ex)
            {
                logger?.LogError($"MySQL error occurred while closing the connection: {ex.Message}");
                return errorValues.ConnectionFailed; // Fehlercode für MySQL-bezogenen Fehler
            }
            catch (Exception ex)
            {
                logger?.LogError($"An unknown error occurred while closing the connection: {ex.Message}");
                return errorValues.UnknownError; // Fehlercode für unbekannte Fehler
            }
            finally
            {
                logger?.LogInformation("closeConnection method completed.");
            }
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
            logger.LogInformation("SELECT Methode gestartet");
            try
            {
                DataTable dt = new DataTable();
                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    openConnection();
                    logger.LogInformation("Verbindung war geschlossen und wird geöffnet");
                }

                // SQL-Abfrage-String zusammensetzen
                if(string.IsNullOrEmpty(column) || string.IsNullOrEmpty(tableName))
                {
                    returnVal = errorValues.EmptyInputParameters;
                    logger.LogWarning("Spalten- oder Tabllenname ist leer");
                }
                else
                {
                    querySelect = $" SELECT {column} FROM {tableName}";
                    logger.LogDebug($"column: [{column}] und tableName: [{tableName}] in Sql Statement eingesetzt");

                }

                // WHERE-Bedingung hinzufügen falls vorhanden
                if (!string.IsNullOrEmpty(whereCondition))
                {
                    querySelect += $" WHERE {whereCondition}";
                    logger.LogDebug($"WHERE Condition hinzugefügt [{whereCondition}]");
                }

                // ORDER BY hinzufügen falls vorhanden
                if (!string.IsNullOrEmpty(orderBy))
                {
                    querySelect += $" ORDER BY {orderBy}";
                    logger.LogDebug($"ORDER BY hinzugefügt: [{orderBy}] ");

                }

                // MySqlCommand erstellen und Abfrage ausführen nur wenn es eine Query gibt
                if (!string.IsNullOrEmpty(querySelect))
                {
                    using (MySqlCommand cmd = new MySqlCommand(querySelect, connection))
                    {
                        logger.LogDebug($"MySqlCommand mit [{querySelect}] wird an MySqlDataReader übergeben");
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            logger.LogInformation($"der Befehl wurde ausgelesen und wird in den DataTable geladen");
                            // Lade die Spaltenstruktur des DataReaders in den DataTable
                            dt.Load(reader);
                        }
                    }
                }
                // DataTable hat keinen Inhalt
                if (dt.Rows.Count == 0)
                {
                    returnVal = errorValues.NoData;
                    logger.LogWarning("Es gibt keine Daten aus der Select-Anfrage");
                }

                logger.LogInformation("Wird über Data Table ausgegeben");
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
                logger.LogWarning($"Es ist ein unbekannter Fehler aufgetreten[{e}]");
            }

            finally
            {
            }
            logger.LogDebug($"Status von SELECT vor beenden: {returnVal}");
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
            logger.LogInformation("UPDATE Methode gestartet");
             errorValues returnVal = errorValues.Success;
            string queryUpdate = "";
            try
            {
                // Sicherstellen, dass der tableName und das Set-Statement nicht leer sind (da Pflicht)
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(set))
                {
                    returnVal = errorValues.EmptyInputParameters;
                    logger.LogWarning("tableName oder set-Wert ist leer");
                }

                // Grundlegende MySql-Abfrage erstellen sofern tableName und set string-Werte enthalten
                else
                {
                    queryUpdate = $" UPDATE {tableName}";

                    // SET-Teil der Abfrage hinzufügen
                    queryUpdate += $" SET {set}";
                    logger.LogDebug($"tableName: [{tableName}] und set: [{set}] in Sql Statement eingesetzt");

                }

                // JOIN hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(join))
                {
                    queryUpdate += $" {join}";
                    logger.LogDebug($"JOIN hinzugefügt: [{join}]");
                }


                // WHERE-Bedingung hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(whereCondition))
                {
                    queryUpdate += $" WHERE {whereCondition}";
                    logger.LogDebug($"WHERE Condition´hinzugefügt: [{whereCondition}]");
                }

                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    logger.LogInformation("Verbindung war geschlossen und wird geöffnet");
                    openConnection();
                }

                if (!string.IsNullOrEmpty(queryUpdate)) { 

                // MySql-Befehl ausführen wenn die Query nicht leer ist
                using (MySqlCommand cmd = new MySqlCommand(queryUpdate, connection))
                {
                        logger.LogDebug($"MySqlCommand [{queryUpdate}] wird ausgeführt");
                        int affectedRows = cmd.ExecuteNonQuery(); // Führt den Befehl aus und gibt die Anzahl der betroffenen Zeilen zurück
                    returnVal = affectedRows > 0 ? errorValues.Success : errorValues.NoData; // Setzt den Enum basierend auf dem Ergebnis
                }
            }
            }
            // Es ist ein anderer Fehler aufgetreten
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
                logger.LogWarning($"Es ist ein unbekannter Fehler aufgetreten [{e}]");
            }

            finally 
            { 
            }

            logger.LogInformation($"Status von UPDATE vor beenden: {returnVal}");
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
            logger.LogInformation("INSERT gestartet");
            // Speichert die Fehlermeldungen in Form von enum Werten
            errorValues returnVal = errorValues.Success;
            try
            {
                string queryInsert = "";
                // Sicherstellen, dass der tableName und values nicht leer sind
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(values))
                {
                    returnVal = errorValues.EmptyInputParameters;
                    logger.LogWarning("tableName oder values-Wert leer");

                }
                // Grundlegende MySql-Abfrage erstellen, sofern table name und values nicht leer sind
                else
                {
                    queryInsert = $" INSERT INTO {tableName}";
                    logger.LogDebug($"tableName hinzugefügt: [{tableName}]");
                }


                // WHERE-Bedingung hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(values))
                {
                    queryInsert += $" VALUES({values});";
                    logger.LogDebug($"VALUES hinzugefügt: [{values}]");
                }


                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {
                    logger.LogInformation("Verbindung war geschlossen und wird geöffnet");
                    openConnection();

                }

                if (!string.IsNullOrEmpty(queryInsert))
                {
                    // MySql-Befehl ausführen, wenn Query nicht leer ist
                    using (MySqlCommand cmd = new MySqlCommand(queryInsert, connection))
                    {
                        logger.LogDebug($"MySqlCommand [{queryInsert}] wird ausgeführt");
                        int affectedRows = cmd.ExecuteNonQuery(); // Führt den Befehl aus und speichert die Anzahl der betroffenen Zeilen
                        returnVal = affectedRows < 0 ? errorValues.Success : errorValues.NoData; //
                    }
                }
            }
            // Es ist ein anderer Fehler aufgetreten
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
                logger.LogWarning($"Es ist ein unbekannter Fehler aufgetreten {e}");
            }

            finally
            {
            }

            logger.LogInformation($"Status von INSERT vor beenden: {returnVal}");
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
            logger.LogInformation("DELETE gestartet");
            errorValues returnVal = errorValues.Success;
            try
            {
                string queryDelete = "";
                // Sicherstellen, dass der tableName und die whereCondition nicht leer sind
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(whereCondition))
                {
                    returnVal = errorValues.EmptyInputParameters;
                    logger.LogWarning($"tableName: [{tableName}] oder WHERE condition: [{whereCondition}] leer");

                                    }
                // Grundlegende MySql-Abfrage erstellen, sofern table name und values nicht leer sind
                else
                {
                    queryDelete = $" DELETE FROM {tableName} WHERE {whereCondition}";
                    logger.LogDebug($"tableName: [{tableName}] und WHERE condiotion: [{whereCondition}] eingesetzt");
                }

                // WHERE-Bedingung hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(limit))
                {
                    queryDelete += $" LIMIT {limit};";
                    logger.LogDebug($"LIMIT eingesetzt: [{limit}]");
                }

                // Sicherstellen, dass die Verbindung geöffnet ist
                if (connection.State == ConnectionState.Closed)
                {

                    logger.LogInformation("Verbindung war geschlossen und wird geöffnet");
                    openConnection();
                }

                // MySql-Befehl ausführen
                if (string.IsNullOrEmpty(queryDelete))
                {
                    using (MySqlCommand cmd = new MySqlCommand(queryDelete, connection))
                    {

                        logger.LogDebug($"MySqlCommand [{queryDelete}] wird ausgeführt");
                        int affectedRows = cmd.ExecuteNonQuery(); // Führt den Befehl aus und speichert die Anzahl der betroffenen Zeilen
                        returnVal = affectedRows > 0 ? errorValues.Success : errorValues.NoData;
                    }
                }
            }
            // allgemeiner Fehler
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
                logger.LogWarning($"Es ist ein unbekannter Fehler aufgetreten: [{e}]");
            }
            finally
            {
            }
            logger.LogInformation($"Status von DELETE vor beenden: [{returnVal}]");
            return returnVal;
        }

        ~MySqlAccess()
        {
            closeConnection();
        }
    }
}
