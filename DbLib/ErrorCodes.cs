
/// Represents an error state where the connection is invalid or corrupted.
/// This error usually indicates that the connection object or state is not in a valid
/// state to perform operations. It typically suggests an issue such as improper
/// initialization, tampering, or external interference during usage.
/// This value is part of the errorValues enum, corresponding to -9.
public enum errorValues
{
    /// <summary>
    /// Represents a successful operation or outcome.
    /// </summary>
    Success = 0, // Erfolg

    // Allgemeine Fehler
    /// <summary>
    /// Represents an error code indicating that an unknown error occurred.
    /// </summary>
    UnknownError = -1,                // Unbekannter Fehler

    /// <summary>
    /// Represents an error that is related to query execution in the database context.
    /// This error indicates that an issue occurred while processing a database query,
    /// such as invalid syntax, incorrect parameters, or other query-related problems.
    /// </summary>
    /// <remarks>
    /// This constant is defined in the <c>errorValues</c> enumeration and can
    /// be returned by methods in the <c>IConnector</c> interface
    /// to signify a query-specific error.
    /// </remarks>
    QueryError = -2,                  // MySQL Query fehlerhaft

    /// Represents an error condition where a connection attempt to a resource or server has failed.
    /// This error
    ConnectionFailed = -3,            // Verbindung zur Datenbank nicht m?glich

    // Verbindungsprobleme
    /// Represents an error that occurs due to a connection-query related issue.
    /// This error indicates that a query could not be executed successfully
    /// due to a problem associated with an active database connection.
    /// It corresponds to the error code -4 in the `errorValues` enumeration.
    /// This error can surface in scenarios where the connection to the database
    /// is valid, but the query execution fails due to reasons such as
    /// improperly maintained connection states, mismatched query execution flow,
    /// or issues arising from dependencies between connection and query operations.
    ConnectionQueryError = -4,        // Verbindungszeichenfolge fehlerhaft

    /// <summary>
    /// Indicates that the server connection attempt has failed.
    /// </summary>
    /// <remarks>
    /// This error value is specifically used to report issues when the system is unable to
    /// successfully establish a connection with the server. It may occur due to network
    /// issues, incorrect server address, or server availability problems.
    /// </remarks>
    ServerConnectionFailed = -5,      // Server konnte nicht erreicht werden (z. B. Ping fehlgeschlagen)

    /// <summary>
    /// Represents an error indicating that authentication has failed.
    /// </summary>
    /// <remarks>
    /// This error is typically returned when the system is unable to authenticate a user or access due to
    /// incorrect credentials, expired sessions, or other authentication-related issues.
    /// </remarks>
    AuthenticationFailed = -6,        // Authentifizierungsfehler (falscher Benutzername/Passwort)

    /// <summary>
    /// Represents an error code indicating that a specified database could not be found.
    /// </summary>
    /// <remarks>
    /// This code is used to signal that the database requested in a query or operation
    /// does not exist or could not be located. Applications can utilize this error
    /// to identify issues with database names or configurations.
    /// </remarks>
    DatabaseNotFound = -7,            // Angegebene Datenbank nicht gefunden

    /// <summary>
    /// Represents an error where an operation cannot be performed because the connection is already closed.
    /// This may occur if a method requiring an active connection is invoked after the connection has been explicitly closed
    /// or was not properly established in the first place.
    /// </summary>
    ConnectionAlreadyClosed = -8,     // Verbindung ist bereits geschlossen

    /// Represents an error state where the connection is invalid or corrupted.
    /// This error usually indicates that the connection object or state is not in a valid
    /// state to perform
    ConnectionInvalid = -9,           // Verbindung ung?ltig (z. B. NULL)

    // Datenprobleme
    /// <summary>
    /// Represents an error returned when no data was found or retrieved during the execution of a query or operation.
    /// </summary>
    NoData = -10,                     // Keine Daten gefunden

    /// <summary>
    /// Indicates an error caused by empty or missing required input parameters.
    /// This error occurs when the provided arguments for a method or operation
    /// are null, empty, or otherwise invalid due to a lack of necessary input.
    /// </summary>
    EmptyInputParameters = -11,       // Ung?ltige oder leere Eingabeparameter

    /// <summary>
    /// Represents an error that occurs when a
    TransactionFailed = -12,          // Transaktion fehlgeschlagen

    /// <summary>
    /// Represents an error indicating that an attempt was made to insert or create a duplicate entry in a database or other data structure.
    /// </summary>
    /// <remarks>
    /// This error is typically returned when a uniqueness constraint, such as a primary key or unique index, is violated by an insert or update operation.
    /// The context in which this error occurs will depend on the implementation of the database or data structure being used.
    /// </remarks>
    DuplicateEntry = -13,             // Eindeutigkeit verletzt (z. B. PRIMARY KEY oder UNIQUE)

    /// <summary>
    /// Represents an error value indicating a syntax error encountered during the execution of a database operation.
    /// </summary>
    /// <remarks>
    /// This error generally occurs when the syntax of a query or command is invalid, such as missing clauses,
    /// mismatched or inappropriate keywords, or unstructured queries.
    /// </remarks>
    SyntaxError = -14,                // SQL-Syntaxfehler

    /// <summary>
    /// Represents an error state where a timeout has occurred during the execution of an operation.
    /// </summary>
    /// <remarks>
    /// This error typically indicates that the operation did not complete within the allotted time.
    /// It can occur in various situations such as connecting to a database, executing a query, or
    /// waiting for an external resource to respond.
    /// </remarks>
    Timeout = -15,                    // Timeout beim Verbindungsaufbau oder SQL-Abfrage

    /// <summary>
    /// Represents an error value indicating that the operation failed due to insufficient permissions.
    /// </summary>
    /// <remarks>
    /// This error occurs when a user or process attempts to perform an action
    /// that requires higher privileges or access rights than those currently granted.
    /// It may be caused by incorrect user
    PermissionDenied = -16,           // Unzureichende Berechtigungen f?r Operation

    /// <summary>
    /// Represents a database constraint violation error.
    /// </summary>
    /// <remarks>
    /// This value is used to indicate that a database operation failed due
    /// to the violation of a constraint, such as primary key, foreign key,
    /// unique constraint, or check constraint.
    /// </remarks>
    ConstraintViolation = -17,        // Verletzung von Fremdschl?ssel- oder anderen Constraints

    /// <summary>
    /// Indicates that the maximum allowed number of connections to the database server
    /// has been reached.
    /// This error occurs when no more connections can be established due to
    /// server-imposed limitations or resource constraints.
    /// </summary>
    TooManyConnections = -18,         // Maximale Anzahl an DB-Verbindungen ?berschritten

    /// <summary>
    /// Indicates that the provided data exceeds the allowed size or length for a database field,
    /// resulting in truncation of the data.
    /// </summary>
    DataTruncation = -19,             // Daten wurden abgeschnitten (z. B. beim INSERT/UPDATE)

    /// <summary>
    /// Represents an error value indicating that a query parameter provided in the database operation
    /// is invalid or does not conform to the expected format or constraints.
    /// </summary>
    /// <remarks>
    /// This error is typically returned when the provided query parameter cannot be processed
    /// due to improper syntax, missing required fields, or invalid data types. The cause may involve
    /// user input errors or programmatic issues in constructing the query.
    /// </remarks>
    InvalidQueryParameter = -20,      // Ung?ltige oder fehlende Query-Parameter

    /// <summary>
    /// Indicates an invalid or improperly formatted LIMIT clause in a query.
    /// </summary>
    /// <remarks>
    /// This error may occur when the LIMIT clause in a SQL query contains
    /// invalid parameters, such as non-numeric values, negative values,
    /// or an incorrect syntax. It signifies that the input for the LIMIT
    /// clause does not meet the expected standards or structure.
    /// </remarks>
    InvalidLimitClause = -21,         // Fehlerhafte oder ung?ltige LIMIT-Klausel

    /// <summary>
    /// Indicates that the specified table was not found in the database during an operation.
    /// This error may occur when the table name provided in a query or command does not exist
    /// or is incorrectly specified.
    /// </summary>
    TableNotFound = -22,              // Tabelle nicht gefunden

    /// <summary>
    /// Represents an error indicating that the specified column could not be found in the database table.
    /// </summary>
    /// <remarks>
    /// This error may occur when attempting to access a database column that does not exist
    /// in the specified table. Common causes for this error include typographical mistakes
    /// in column names, column not being present in the schema, or missing mappings
    /// between the database and the application.
    /// </remarks>
    ColumnNotFound = -23,             // Spalte nicht gefunden

    /// <summary>
    /// Represents an error that occurs when a connection attempt to the database exceeds the allocated time limit.
    /// This typically indicates that the database server is unresponsive or the network latency is too high to establish a connection in a timely manner.
    /// </summary>
    ConnectionTimeout = -24,          // Verbindung zur Datenbank hat ein Timeout erreicht

    /// <summary>
    /// Represents a network-related error encountered during communication
    /// between the application and a remote server or database.
    /// This error is typically raised when there are issues such as network
    /// disruptions, misconfigured network settings, or other underlying
    /// problems preventing successful data transmission.
    /// </summary>
    NetworkError = -25,               // Netzwerkfehler beim Versuch, die Datenbank zu erreichen

}

