using System;
using System.Data;
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

        private readonly ILogger<MySqlAccess>? logger;
        private readonly MySqlConnection connection;

        public errorValues flagStatus = errorValues.Success;  // Verbindungsstatus



        /// <summary>
        /// Der Konstruktor öffnet die Connection und speichert den return-Wert im flagStatus, damit man sehen kann ob die Verbindung aufgebaut wurde. Es wird kein Logger benötigt
        /// </summary>

        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MySqlAccess(string server, string database, string username, string password)
        {

        {
            // Überprüfen, ob die Verbindungsparameter gültig sind
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                flagStatus = errorValues.EmptyInputParameters;
                return;
            }

            try
            {
                // Verbindung erstellen
                var connectionString = $"Server={server};Database={database};User ID={username};Password={password};";
                connection = new MySqlConnection(connectionString);

                // Verbindung öffnen
                flagStatus = openConnection();
            }
            catch (MySqlException ex)
            {
                logger?.LogError(ex, "Fehler bei der Erstellung der MySQL-Verbindung.");
                flagStatus = errorValues.UnknownError;
            }
        }
        }




        /// <summary>
        /// Der Konstruktor öffnet die Connection und speichert den return-Wert im flagStatus, damit man sehen kann ob die Verbindung aufgebaut wurde
        /// </summary>
        
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MySqlAccess(string server, string database, string username, string password, ILogger<MySqlAccess>? logger)
        {
            // Überprüfen, ob die Verbindungsparameter gültig sind
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                flagStatus = errorValues.EmptyInputParameters;
                return;
            }

            // Logger initialisieren
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                // Verbindung erstellen
                var connectionString = $"Server={server};Database={database};User ID={username};Password={password};";
                connection = new MySqlConnection(connectionString);

                // Verbindung öffnen
                flagStatus = openConnection();
            }
            catch (MySqlException ex)
            {
                logger?.LogError(ex, "Fehler bei der Erstellung der MySQL-Verbindung.");
                flagStatus = errorValues.UnknownError;
            }
        }


        /// <summary>
        /// Der Konstruktor öffnet die Connection und speichert den return-Wert im flagStatus, damit man sehen kann ob die Verbindung aufgebaut wurde
        /// </summary>
        /// 

        /// <param name="connection"></param>
        /// <param name="logger"></param>
        /// 
        /// <flagStatus>
        /// - Success: Verbindung erfolgreich hergestellt.
        /// - EmptyInputParameters: Einer der Pflichtparameter fehlt.
        /// </flagStatus>



        // @override
        public MySqlAccess(MySqlConnection connection, ILogger<MySqlAccess>? logger)

        {
            // Überprüfen ob die Verbindungsparameter enthalten sind
            // flagStatus errorValues.emptyParameters
            if (connection == null)
            {
                flagStatus = errorValues.EmptyInputParameters;  
                return;
            }

            // Verbindung herstellen
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Verbindung öffnen 
            flagStatus = openConnection();

        }

        /// <summary> 
        /// Versucht die Verbindung zur Datenbank über den MySql Connector herzustellen.
        /// Falls die Verbindung schon besteht, wird der Prozess frühzeitig beendet.
        /// </summary>
        /// 
        /// <returns>
        /// - Success: Verbindung wurde erfolgreich geöffnet.
        /// - ConnectionInvalid: Die Verbindung ist in einem ungültigen Zustand.
        /// - AuthenticationFailed: Authentifizierungsfehler (z. B. falscher Benutzername oder Passwort).
        /// - DatabaseNotFound: Die angegebene Datenbank existiert nicht.
        /// - ServerConnectionFailed: Der Server konnte nicht erreicht werden.
        /// - ConnectionQueryError: Ein MySQL-Fehler ist während der Verbindungsherstellung aufgetreten.
        /// - UnknownError: Ein unbekannter Fehler ist aufgetreten.
        /// </returns>
        public errorValues openConnection()
        {
            logger?.LogInformation($"Attempting to open connection to: {connection.Database}");

            try
            {
                // Verbindung öffnen, falls sie nicht bereits offen ist
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    logger?.LogInformation("Connection already open.");
                    return errorValues.Success;
                }
                else
                {
                    // Versuche, die Verbindung zu öffnen
                    connection.Open();
                    logger?.LogInformation("Connection opened successfully.");
                    return errorValues.Success;

                }

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
            catch (MySqlException ex) when (ex.Number == 1042) // Server nicht erreichbar
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
        /// Schließt die bestehende Datenbankverbindung.
        /// Rückgabewerte:
        /// - Success: Verbindung wurde erfolgreich geschlossen.
        /// - ConnectionInvalid: Die Verbindung ist ungültig oder null.
        /// - ConnectionAlreadyClosed: Die Verbindung war bereits geschlossen.
        /// - ConnectionFailed: Ein MySQL-bezogener Fehler ist beim Schließen der Verbindung aufgetreten.
        /// - UnknownError: Ein unbekannter Fehler ist aufgetreten.
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
        /// - Success: Abfrage erfolgreich, Daten wurden gefunden und verarbeitet.
        /// - NoData: Abfrage erfolgreich, aber es wurden keine Datensätze gefunden.
        /// - EmptyInputParameters: Einer der Pflichtparameter fehlt (column oder tableName).
        /// - QueryError: Fehler bei der SQL-Abfrage (z. B. Syntaxfehler, Datenbankprobleme).
        /// - UnknownError: Ein unbekannter Fehler ist aufgetreten.
        /// </returns>
        public errorValues select(string column, string tableName, string whereCondition = "", string orderBy = "")
        {
            errorValues returnVal = errorValues.Success;
            string querySelect = "";
            logger.LogInformation("SELECT Methode gestartet");

            try
            {
                // Sicherstellen, dass Eingabeparameter vorhanden sind
                if (string.IsNullOrEmpty(column) || string.IsNullOrEmpty(tableName))
                {
                    throw new ArgumentException("Spalten- oder Tabellenname ist leer.");
                }

                // Verbindung öffnen, falls sie geschlossen ist
                if (connection.State == ConnectionState.Closed)

                {
                    logger?.LogInformation("Verbindung war geschlossen und wird geöffnet.");
                    openConnection();
                }

                // SQL-Abfrage-String zusammensetzen
                querySelect = $"SELECT {column} FROM {tableName}";
                logger?.LogDebug($"column: [{column}] und tableName: [{tableName}] in SQL-Statement eingesetzt");

                // WHERE-Bedingung hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(whereCondition))
                {
                    querySelect += $" WHERE {whereCondition}";
                    logger?.LogDebug($"WHERE Condition hinzugefügt [{whereCondition}]");
                }

                // ORDER BY hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(orderBy))
                {
                    querySelect += $" ORDER BY {orderBy}";
                    logger?.LogDebug($"ORDER BY hinzugefügt: [{orderBy}]");
                }

                // MySqlCommand erstellen und Abfrage ausführen
                DataTable dt = new DataTable();
                using (MySqlCommand cmd = new MySqlCommand(querySelect, connection))
                {
                    logger.LogDebug($"MySqlCommand mit [{querySelect}] wird an MySqlDataReader übergeben.");
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        logger?.LogInformation("Der Befehl wurde ausgelesen und wird in den DataTable geladen.");
                        dt.Load(reader);
                    }
                }

                // Prüfen, ob der DataTable leer ist
                if (dt.Rows.Count == 0)
                {
                    throw new InvalidOperationException("Es gibt keine Daten aus der SELECT-Anfrage.");
                }

                // DataTable ausgeben (optional)
                logger?.LogInformation("Wird über Data Table ausgegeben:");
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        Console.Write($"{row[col]} | ");
                    }
                    Console.WriteLine();
                }
            }
            catch (ArgumentException e)
            {
                returnVal = errorValues.EmptyInputParameters;
                logger?.LogWarning($"Fehlerhafte Eingabeparameter: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                returnVal = errorValues.NoData;
                logger?.LogWarning($"Keine Daten gefunden: {e.Message}");
            }
            catch (MySqlException e)
            {
                returnVal = errorValues.QueryError;
                logger?.LogError($"Datenbankfehler: {e.Message}");
            }
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
                logger?.LogError($"Unbekannter Fehler: {e.Message}");
            }
            finally
            {
                logger?.LogDebug($"Status von SELECT vor Beenden: {returnVal}");
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
        /// - Success: Abfrage erfolgreich, mindestens ein Datensatz wurde aktualisiert.
        /// - NoData: Abfrage erfolgreich, aber keine Datensätze wurden aktualisiert.
        /// - EmptyInputParameters: Einer der Pflichtparameter fehlt (tableName oder set).
        /// - InvalidQueryParameter: Fehler bei der Generierung der SQL-Abfrage.
        /// - QueryError: Fehler bei der SQL-Abfrage (z. B. Syntaxfehler).
        /// - UnknownError: Ein unbekannter Fehler ist aufgetreten.
        /// </returns>

        public errorValues update(string tableName, string set, string whereCondition = "", string join = "")
        {
            logger?.LogInformation("UPDATE Methode gestartet");
            errorValues returnVal = errorValues.Success;
            string queryUpdate = "";

            try
            {
                // Grundlegende MySQL-Abfrage erstellen
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(set))
                {
                    throw new ArgumentException("tableName oder set-Wert ist leer");
                }

                queryUpdate = $"UPDATE {tableName} SET {set}";
                logger?.LogDebug($"tableName: [{tableName}] und set: [{set}] in SQL Statement eingesetzt");

                // JOIN hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(join))
                {
                    queryUpdate += $" {join}";
                    logger?.LogDebug($"JOIN hinzugefügt: [{join}]");
                }

                // WHERE-Bedingung hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(whereCondition))
                {
                    queryUpdate += $" WHERE {whereCondition}";
                    logger?.LogDebug($"WHERE Condition hinzugefügt: [{whereCondition}]");
                }

                // Verbindung öffnen, falls sie geschlossen ist
                if (connection.State == ConnectionState.Closed)
                {
                    logger?.LogInformation("Verbindung war geschlossen und wird geöffnet");
                    openConnection();
                }

                if (string.IsNullOrEmpty(queryUpdate))
                    throw new InvalidOperationException("QueryUpdate konnte nicht generiert werden");

                // MySQL-Befehl ausführen
                using (MySqlCommand cmd = new MySqlCommand(queryUpdate, connection))
                {
                    logger?.LogDebug($"MySQL Command [{queryUpdate}] wird ausgeführt");
                    int affectedRows = cmd.ExecuteNonQuery(); // Führt den Befehl aus und gibt die Anzahl der betroffenen Zeilen zurück
                    returnVal = affectedRows > 0 ? errorValues.Success : errorValues.NoData; // Basierend auf den Änderungen Erfolg oder NoData
                }
            }
            catch (ArgumentException e)
            {
                returnVal = errorValues.EmptyInputParameters;
                logger?.LogWarning($"Ungültige Eingabeparameter: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                returnVal = errorValues.InvalidQueryParameter;
                logger?.LogWarning($"Fehler bei der Query-Generierung: {e.Message}");
            }
            catch (MySqlException e)
            {
                returnVal = errorValues.QueryError;
                logger?.LogError($"Fehler beim Ausführen der MySQL-Abfrage: {e.Message}");
            }

            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
                logger?.LogError($"Ein unbekannter Fehler ist aufgetreten: {e.Message}");
            }
            finally
            {
                // Verbindung schließen, falls nötig (optional, wenn gewünscht)
                if (connection.State != ConnectionState.Closed)
                {
                    logger?.LogInformation("Schließe die Verbindung");
                    connection.Close();
                }

            }

            logger?.LogInformation($"Status von UPDATE vor Beenden: {returnVal}");
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
        /// - Success: Abfrage erfolgreich, mindestens ein Datensatz eingefügt.
        /// - NoData: Abfrage erfolgreich, aber keine Datensätze eingefügt.
        /// - EmptyInputParameters: Einer der Pflichtparameter fehlt.
        /// - DuplicateEntry: Duplikatfehler (z. B. PRIMARY KEY verletzt).
        /// - ConstraintViolation: Constraint-Verletzung (z. B. NOT NULL verletzt).
        /// - TableNotFound: Die angegebene Tabelle existiert nicht.
        /// - QueryError: Fehler bei der SQL-Abfrage.
        /// - UnknownError: Ein unbekannter Fehler ist aufgetreten.
        /// </returns>
        public errorValues insert(string tableName, string values)
        {
            logger?.LogInformation("INSERT gestartet");
            errorValues returnVal = errorValues.Success;
            string queryInsert = "";
            try
            {
                // Überprüfen, ob tableName und values gültig sind
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(values))
                    throw new ArgumentException("tableName oder values-Wert ist leer");

                // Grundlegende MySQL-Abfrage erstellen
                queryInsert = $"INSERT INTO {tableName} VALUES({values});";
                logger?.LogDebug($"SQL-Query erstellt: {queryInsert}");

                // Verbindung öffnen, falls sie geschlossen ist
                if (connection.State == ConnectionState.Closed)
                {
                    logger?.LogInformation("Verbindung war geschlossen und wird geöffnet");
                    openConnection();
                }

                // MySQL-Befehl ausführen
                using (MySqlCommand cmd = new MySqlCommand(queryInsert, connection))
                {
                    logger?.LogDebug($"MySQL Command wird ausgeführt: {queryInsert}");
                    int affectedRows = cmd.ExecuteNonQuery();
                    returnVal = affectedRows > 0 ? errorValues.Success : errorValues.NoData;
                }
            }
            catch (ArgumentException e)
            {
                returnVal = errorValues.EmptyInputParameters;
                logger?.LogWarning($"Ungültige Eingabeparameter: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                returnVal = errorValues.InvalidQueryParameter;
                logger?.LogWarning($"Fehler bei der Query-Generierung: {e.Message}");
            }
            catch (MySqlException e) when (e.Number == 1062) // Duplikatwert (PRIMARY KEY oder UNIQUE verletzt)
            {
                returnVal = errorValues.DuplicateEntry;
                logger?.LogError($"Duplikatfehler bei Einfügen: {e.Message}");
            }
            catch (MySqlException e) when (e.Number == 1048) // NOT NULL Constraint verletzt
            {
                returnVal = errorValues.ConstraintViolation;
                logger?.LogError($"Datenconstraint verletzt: {e.Message}");
            }
            catch (MySqlException e) when (e.Number == 1146) // Tabelle nicht gefunden
            {
                returnVal = errorValues.TableNotFound;
                logger?.LogError($"Tabelle nicht gefunden: {e.Message}");
            }
            catch (MySqlException e) // Allgemeine MySQL-Fehler
            {
                returnVal = errorValues.QueryError;
                logger?.LogError($"Fehler bei der MySQL-Abfrage: {e.Message}");
            }
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
                logger?.LogError($"Ein unbekannter Fehler ist aufgetreten: {e.Message}");
            }
            finally
            {
                // Verbindung schließen, falls sie noch geöffnet ist
                if (connection.State != ConnectionState.Closed)
                {
                    logger?.LogInformation("Schließe die Verbindung");
                    connection.Close();
                }
            }

            logger?.LogInformation($"Status von INSERT vor Beenden: {returnVal}");
            return returnVal;
        }



        /// <summary>
        /// Führt eine SQL DELETE-Abfrage aus, basierend auf den übergebenen Parametern.
        /// Die Methode validiert die Eingaben, erstellt eine vollständige SQL-Abfrage und führt diese aus.
        /// Bei Fehlern werden entsprechende Fehlercodes zurückgegeben.
        /// </summary>
        /// 
        /// <param name="tableName">
        /// <param name="whereCondition">
        /// <param name="limit">
        /// 
        /// <returns>
        /// Mögliche Rückgabewerte:
        /// - Success: Abfrage erfolgreich.
        /// - NoData: Keine Datensätze gelöscht.
        /// - EmptyInputParameters: Eingabewerte fehlen.
        /// - TableNotFound: Tabelle existiert nicht.
        /// - QueryError: Fehler bei der SQL-Abfrage.
        /// - UnknownError: Ein unbekannter Fehler ist aufgetreten.
        /// </returns>

        public errorValues delete(string tableName, string whereCondition, string limit = "")
        {
            logger?.LogInformation("DELETE gestartet");
            errorValues returnVal = errorValues.Success;
            string queryDelete = "";

            try
            {
                // Überprüfen, ob tableName und whereCondition gültig sind
                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(whereCondition))
                    throw new ArgumentException("tableName oder WHERE-Bedingung ist leer");

                // Grundlegende MySQL-Abfrage erstellen
                queryDelete = $"DELETE FROM {tableName} WHERE {whereCondition}";
                logger?.LogDebug($"SQL-Query erstellt: {queryDelete}");


                // LIMIT hinzufügen, falls vorhanden
                if (!string.IsNullOrEmpty(limit))
                {
                    queryDelete += $" LIMIT {limit};";
                    logger?.LogDebug($"LIMIT hinzugefügt: {limit}");
                }

                // Verbindung öffnen, falls sie geschlossen ist
                if (connection.State == ConnectionState.Closed)
                {
                    logger?.LogInformation("Verbindung war geschlossen und wird geöffnet");
                    openConnection();
                }

                // MySQL-Befehl ausführen
                using (MySqlCommand cmd = new MySqlCommand(queryDelete, connection))
                {
                    logger?.LogDebug($"MySQL Command wird ausgeführt: {queryDelete}");
                    int affectedRows = cmd.ExecuteNonQuery();
                    returnVal = affectedRows > 0 ? errorValues.Success : errorValues.NoData;
                }
            }
            catch (ArgumentException e)
            {
                returnVal = errorValues.EmptyInputParameters;
                logger?.LogWarning($"Ungültige Eingabeparameter: {e.Message}");
            }
            catch (MySqlException e) when (e.Number == 1146) // Tabelle nicht gefunden
            {
                returnVal = errorValues.TableNotFound;
                logger?.LogError($"Tabelle nicht gefunden: {e.Message}");
            }
            catch (MySqlException e) when (e.Number == 1091) // Spalte oder Bedingung nicht gefunden
            {
                returnVal = errorValues.ColumnNotFound;
                logger?.LogError($"Spalte oder WHERE-Bedingung nicht gefunden: {e.Message}");
            }
            catch (MySqlException e) when (e.Number == 1048) // NOT NULL Constraint verletzt
            {
                returnVal = errorValues.ConstraintViolation;
                logger?.LogError($"Constraint-Verletzung: {e.Message}");
            }
            catch (MySqlException e) // Allgemeine MySQL-Fehler
            {
                returnVal = errorValues.QueryError;
                logger?.LogError($"Fehler bei der MySQL-Abfrage: {e.Message}");
            }
            catch (Exception e)
            {
                returnVal = errorValues.UnknownError;
                logger?.LogError($"Ein unbekannter Fehler ist aufgetreten: {e.Message}");
            }
            finally
            {
                // Verbindung schließen, falls sie noch geöffnet ist
                if (connection.State != ConnectionState.Closed)
                {
                    logger?.LogInformation("Schließe die Verbindung");
                    connection.Close();
                }
            }

            logger?.LogInformation($"Status von DELETE vor Beenden: {returnVal}");
            return returnVal;
        }


        ~MySqlAccess()
        {
            closeConnection();
        }
    }
}
