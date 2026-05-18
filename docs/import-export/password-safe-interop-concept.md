# Password-Safe-Interop-Konzept

## Ziel

Der SASD Desktop Secret Manager soll vorhandene Password-Safe-/PWSafe-Daten realistisch übernehmen können, ohne das eigene interne Modell auf die Grenzen von `.psafe3` zu reduzieren.

## Grundentscheidung

Password Safe ist eine Interoperabilitätsschicht, nicht das Primärmodell. Das interne `.svault`-Modell bleibt fachlich führend.

## V1: Import

V1 konzentriert sich auf kontrollierten Import:

1. Nutzer wählt `.psafe3`-Datei.
2. Nutzer gibt Password-Safe-Master-Passwort ein.
3. Importkomponente liest die Datei.
4. Mapper erzeugt interne Secret-Manager-Einträge.
5. Importbericht dokumentiert Ergebnis, Warnungen und nicht vollständig abbildbare Inhalte.
6. Importierte Daten werden erst nach bewusster Nutzerentscheidung in einen `.svault`-Tresor übernommen.

## Mapping-Klassen

| Klasse | Beschreibung | Behandlung |
|---|---|---|
| Direkt abbildbar | Titel, Benutzername, Passwort, URL, Notizen, Gruppe | direkte Übernahme in Standardfelder |
| Strukturiert abbildbar | technische Zusatzinformationen, wenn erkennbar | Übernahme als CustomFields |
| Nicht eindeutig abbildbar | fremde Spezialfelder, unbekannte Metadaten | Importbericht und optional gekennzeichneter Metadatenblock |
| Nicht unterstützbar | beschädigte Daten, unlesbare Inhalte | Fehler oder Warnung, kein stiller Verlust |

## Export

Export nach `.psafe3` ist nicht Teil der frühen V1. Er gehört frühestens in V1.x, weil Schreibkompatibilität eine klare Verlust- und Mapping-Strategie benötigt.

## CSV-/Klartextexporte

CSV- oder Klartextexporte sind sicherheitskritisch. Sie dürfen nur nach ausdrücklicher Warnung und bewusster Nutzerentscheidung angeboten werden. Exportdateien müssen als hochkritisch behandelt und nach Migration sicher gelöscht werden.

## Browser-Import

Browser-Importe sind wegen Datenschutz, Browserabhängigkeiten und lokalem Sicherheitsmodell auf V3/später verschoben. Sie dürfen nicht in den V1-Kern gezogen werden.

## Testpflichten

- Import einer kleinen Testdatei.
- Import mit Gruppenstruktur.
- Import mit Sonderzeichen und Umlauten.
- Import mit langen Notizen.
- Import mit nicht abbildbaren Feldern.
- Import mit falschem Passwort.
- Import einer beschädigten Datei.
- Prüfung, dass keine Import-Secrets in Logs erscheinen.
