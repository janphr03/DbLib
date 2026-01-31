# DbLib – MySQL Connector für .NET

DbLib ist eine leichtgewichtige C#/.NET-Bibliothek für den Zugriff auf MySQL-Datenbanken.
Sie bietet eine saubere Abstraktion über Interfaces, eigene Fehlercodes und Tests.

---

## Features

- SQL-Ausführung (SELECT, INSERT, UPDATE, DELETE)
- Connector-Abstraktion über `IConnector`
- Fehlerbehandlung mit `ErrorCodes` (Enums)
- Unit- und Integrationstests (Docker-MySQL)
- Beispiel-Console-App enthalten

---

## Struktur

    DbLib/
    ├── DbLib/              # Bibliothek
    ├── App/                # Beispiel-App
    ├── DbLib.TestUnits/    # Tests
    └── README.md

---

## Voraussetzungen

- .NET 8.0
- MySQL Server (lokal oder Docker)

---

## Setup

Repository klonen:

    git clone https://github.com/janphr03/DbLib.git
    cd DbLib

Umgebungsvariablen setzen (PowerShell):

    $env:MYSQL_SERVER="localhost"
    $env:MYSQL_DATABASE="testprotocol"
    $env:MYSQL_USER="root"
    $env:MYSQL_PASSWORD="deinPasswort"

---

## Tests

Integrationstests benötigen Docker:

    docker ps

---

## Hinweis

Passwörter werden nicht im Code gespeichert.
Bitte keine `.env` oder Secrets in Git committen.

---

Diese ReadMe wurde mit Hilfe von KI erzeugt
