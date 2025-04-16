
public enum errorValues
{
    Success = 0,                      // Erfolg

    // Allgemeine Fehler
    UnknownError = -1,                // Unbekannter Fehler
    QueryError = -2,                  // MySQL Query fehlerhaft
    ConnectionFailed = -3,            // Verbindung zur Datenbank nicht m?glich

    // Verbindungsprobleme
    ConnectionQueryError = -4,        // Verbindungszeichenfolge fehlerhaft
    ServerConnectionFailed = -5,      // Server konnte nicht erreicht werden (z. B. Ping fehlgeschlagen)
    AuthenticationFailed = -6,        // Authentifizierungsfehler (falscher Benutzername/Passwort)
    DatabaseNotFound = -7,            // Angegebene Datenbank nicht gefunden
    ConnectionAlreadyClosed = -8,     // Verbindung ist bereits geschlossen
    ConnectionInvalid = -9,           // Verbindung ung?ltig (z. B. NULL)

    // Datenprobleme
    NoData = -10,                     // Keine Daten gefunden
    EmptyInputParameters = -11,       // Ung?ltige oder leere Eingabeparameter
    TransactionFailed = -12,          // Transaktion fehlgeschlagen
    DuplicateEntry = -13,             // Eindeutigkeit verletzt (z. B. PRIMARY KEY oder UNIQUE)
    SyntaxError = -14,                // SQL-Syntaxfehler
    Timeout = -15,                    // Timeout beim Verbindungsaufbau oder SQL-Abfrage
    PermissionDenied = -16,           // Unzureichende Berechtigungen f?r Operation
    ConstraintViolation = -17,        // Verletzung von Fremdschl?ssel- oder anderen Constraints
    TooManyConnections = -18,         // Maximale Anzahl an DB-Verbindungen ?berschritten
    DataTruncation = -19,             // Daten wurden abgeschnitten (z. B. beim INSERT/UPDATE)
    InvalidQueryParameter = -20,      // Ung?ltige oder fehlende Query-Parameter
    InvalidLimitClause = -21,         // Fehlerhafte oder ung?ltige LIMIT-Klausel
    TableNotFound = -22,              // Tabelle nicht gefunden
    ColumnNotFound = -23,             // Spalte nicht gefunden
    ConnectionTimeout = -24,          // Verbindung zur Datenbank hat ein Timeout erreicht
    NetworkError = -25,               // Netzwerkfehler beim Versuch, die Datenbank zu erreichen

}

