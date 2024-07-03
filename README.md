# Donation Reader

**Donation Reader** ist ein Projekt, das darauf abzielt, alle Spenden über 50.000 €, die an politische Parteien in Deutschland getätigt wurden, von der offiziellen Webseite des Bundestages auszulesen, in einer Datenbank zu speichern und grafisch in einer WPF (Windows Presentation Foundation) Anwendung mit C# darzustellen. 

## Inhaltsverzeichnis
- [Hintergrund](#hintergrund)
- [Funktionen](#funktionen)
- [Installation](#installation)
- [Verwendung](#verwendung)
- [Screenshots](#screenshots)
- [Technologien](#technologien)

## Hintergrund
Ich habe dieses Projekt im Rahmen meines Praktikums begonnen, um meine Kenntnisse in C# und WPF zu verbessern. Das Projekt dient keinem politischen Zweck; es verwendet einfach öffentlich zugängliche Daten, um eine technische Herausforderung zu bewältigen und meine Programmierfähigkeiten weiterzuentwickeln.

## Funktionen
- **Datenextraktion:** Automatisches Auslesen von Spenden über 50.000 € von der Webseite des Bundestages.
- **Datenbank:** Speicherung der ausgelesenen Daten in einer SQLite-Datenbank.
- **Grafische Darstellung:** Visualisierung der Daten in verschiedenen Ansichten innerhalb einer WPF-Anwendung.
    - Übersicht der Spenden
    - Diagramme und Graphen zur Darstellung von Spendentendenzen
    - Detailansicht einzelner Spenden

## Installation
1. **Repository klonen:**
    ```bash
    git clone https://github.com/LukasTrust/donations_reader.git
    cd donations_reader
    ```

2. **Abhängigkeiten installieren:**
    - Stelle sicher, dass .NET Core SDK und Postgres installiert sind.

3. **Projekt kompilieren:**
    Öffne das Projekt in Visual Studio und baue die Lösung.

## Verwendung
1. **Daten abrufen:** Starte die Anwendung und klicke auf den "Daten abrufen" Button, um die neuesten Spenden aus dem Bundestag abzurufen.
2. **Daten anzeigen:** Verwende die verschiedenen Ansichten in der Anwendung, um die abgerufenen Daten grafisch darzustellen.

## Screenshots

## Technologien
- **Programmiersprache:** C#
- **Framework:** .NET Core, WPF
- **Datenbank:** Postgres
- **Libraries:** 
    - LiveCharts (für die Diagramme)
