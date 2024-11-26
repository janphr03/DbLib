# DbLib - MySQL Access Library

**Version:** 1.0.2  
**Autor:** Jan Herrmann  
**Lizenz:** MIT  

---

## **Beschreibung**

DbLib ist eine C#-Bibliothek, die eine einfache und sichere Möglichkeit bietet, mit einer MySQL-Datenbank zu interagieren. Die Bibliothek bietet Funktionen für:

- **Datenbankverbindung** (Herstellen und Schließen von Verbindungen)
- **CRUD-Operationen** (SELECT, INSERT, UPDATE, DELETE)
- Fehlerhandling mit **detaillierten Rückgabewerten** (Enums)

Diese Bibliothek ist ideal für Entwickler, die eine robuste und leicht zu integrierende Lösung für MySQL-Datenbanken benötigen.

---

## **Features**

- **Einfache Handhabung von Verbindungen**: Automatisches Öffnen und Schließen von Verbindungen.
- **Umfassendes Fehlerhandling**: Unterstützt verschiedene Fehlertypen, einschließlich Verbindungsprobleme, Syntaxfehler und Datenintegritätsprobleme.
- **Erweiterbare Architektur**: Die Schnittstelle `IConnector` kann implementiert werden, um andere Datenbanktypen zu unterstützen.
- **Logging-Unterstützung**: Integriert mit `Microsoft.Extensions.Logging`, um detaillierte Logs bereitzustellen.

---

## **Voraussetzungen**

- **.NET Version**: `.NET 6.0` oder höher
- **MySQL Datenbank**: Version 5.7 oder höher
- **NuGet-Pakete**:
  - [MySql.Data](https://www.nuget.org/packages/MySql.Data)
  - [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging)

---

## **Installation**

Installiere das Paket über den NuGet-Paketmanager:

### **NuGet Package Manager**
```bash
Install-Package DbLib


1. Verbindung herstellen

using DbLib;
using Microsoft.Extensions.Logging;

// Logger-Konfiguration
ILogger<MySqlAccess> logger = new LoggerFactory().CreateLogger<MySqlAccess>();

// Verbindung erstellen
var dbAccess = new MySqlAccess("localhost", "myDatabase", "username", "password", logger);

// Verbindungsstatus überprüfen
if (dbAccess.flagStatus == errorValues.Success)
{
    Console.WriteLine("Verbindung erfolgreich hergestellt!");
}
else
{
    Console.WriteLine($"Fehler beim Verbindungsaufbau: {dbAccess.flagStatus}");
}


2. Daten abrufen (SELECT)

var result = dbAccess.select("id, name", "users", "age > 18", "id ASC");
if (result == errorValues.Success)
{
    Console.WriteLine("Daten erfolgreich abgerufen!");
}
else
{
    Console.WriteLine($"Fehler bei der SELECT-Abfrage: {result}");
}

3. Daten einfügen (INSERT)

var result = dbAccess.insert("users", "'John', 'Doe', 30");
if (result == errorValues.Success)
{
    Console.WriteLine("Daten erfolgreich eingefügt!");
}
else
{
    Console.WriteLine($"Fehler beim INSERT: {result}");
}

4. Daten aktualisieren (UPDATE)

var result = dbAccess.update("users", "name = 'Jane Doe'", "id = 1");
if (result == errorValues.Success)
{
    Console.WriteLine("Daten erfolgreich aktualisiert!");
}
else
{
    Console.WriteLine($"Fehler beim UPDATE: {result}");
}


5. Daten löschen (DELETE)

var result = dbAccess.delete("users", "age < 18", "10");
if (result == errorValues.Success)
{
    Console.WriteLine("Daten erfolgreich gelöscht!");
}
else
{
    Console.WriteLine($"Fehler beim DELETE: {result}");
}

Fehlercodes
Success	= Operation erfolgreich.
UnknownError = Ein unbekannter Fehler ist aufgetreten.
QueryError= Fehler in der SQL-Abfrage.
NoData = Keine Daten gefunden.
EmptyInputParameters = Ungültige oder leere Eingabeparameter.
DatabaseNotFound = Die angegebene Datenbank wurde nicht gefunden.
DuplicateEntry = Duplikatfehler (PRIMARY KEY/UNIQUE verletzt).
TableNotFound = Die angegebene Tabelle existiert nicht.

MIT License

Copyright (c) [2024] [Jan Herrmann]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
