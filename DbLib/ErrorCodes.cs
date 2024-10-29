
    public enum errorValues
    {
        Success = 0,
        UnknownError = -1,          // unbekannterFehler
        QueryError = -2,            // MySql Query fehlerhaft
        ConnectionFailed = -3,      // Verbindung zur DB nicht möglich
        ConnectionQueryError = -4,  // verbindungszeichnfolge
        ServerConnectionFailed = -5,// Server konnte nicht gepingt werden
        ConnectionAlreadClosed = -6,// Verbindung ist bereits geschlossen 
        ConnectionInvalid = -7,     // Verbindung ist nicht vorhanden z.B. NULL
        NoData = -8,

    }
