# UI-/UX-Konzept

## Zielbild

Die Anwendung soll wie ein ruhiger, professioneller Windows-Desktop-Secret-Manager wirken. Sie soll nicht verspielt, überladen oder cloudartig erscheinen, sondern kontrolliert, nachvollziehbar und sicherheitsbewusst.

## Hauptlayout

```text
+----------------------------------------------------------+
| Menü / Toolbar / Statusinformationen                     |
+-------------------+--------------------+-----------------+
| Tresor / Gruppen  | Suche / Einträge    | Details         |
| TreeView          | ListView / Filter   | Secret-Ansicht  |
+-------------------+--------------------+-----------------+
```

## Organisationsprinzipien

| Konzept | Aufgabe |
|---|---|
| Gruppen | feste organisatorische Ablage |
| Tags | flexible Querverbindungen |
| Zusatzfelder | strukturierte technische Informationen |
| Typen | fachliche Einordnung und UI-Hilfen |

Diese Konzepte dürfen nicht vermischt werden. Ein Tag ersetzt keine Gruppe, und eine Notiz ersetzt kein strukturiertes Zusatzfeld.

## Secret-Anzeige

- Secrets sind standardmäßig maskiert.
- Reveal muss bewusst ausgelöst werden.
- Copy-Aktionen für sensible Werte müssen Feedback geben.
- Clipboard-Autoclear ist für sensible Werte Pflicht.
- Nicht sensible Werte wie Host oder URL können bequemer kopiert werden, müssen aber klar von Secrets unterscheidbar sein.

## Zertifikatsansicht

Für HTTPS-/TLS-Zertifikate ist eine eigene Detailstruktur sinnvoll:

- Common Name,
- Subject Alternative Names,
- Issuer,
- Seriennummer,
- Fingerprints,
- Valid From / Valid To,
- Speicherort oder Referenz,
- Private Key vorhanden: ja/nein,
- PFX/PKCS#12 vorhanden: ja/nein,
- Passphrase als Secret.

Private Key, PFX und Passphrase werden nicht wie normale Textfelder behandelt, sondern wie hochkritische Secrets.

## Kontextmenüs

Kontextmenüs sollen zentrale Arbeitswege unterstützen:

- Neue Gruppe,
- Gruppe umbenennen,
- Gruppe löschen,
- Neuer Eintrag,
- Eintrag bearbeiten,
- Eintrag löschen,
- Wert kopieren,
- Secret anzeigen/verbergen,
- Eintrag verschieben.

## Fehlbedienungsschutz

Riskante Aktionen benötigen Bestätigung:

- Löschen,
- Verschieben nicht leerer Gruppen,
- Import mit Konflikten,
- Export in unsichere Formate,
- Überschreiben bestehender Dateien.

## Nicht-Ziele V1

- Browser-Autofill,
- mobile UI,
- Cloud-Sync-UI,
- Team-Sharing-Oberfläche,
- komplexer Plugin-Marktplatz,
- rich text als Standard für Notizen.
