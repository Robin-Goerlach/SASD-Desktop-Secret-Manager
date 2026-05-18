# Featuremap: SASD Desktop Secret Manager

**Überarbeitete Featuremap passend zu Lastenheft, Pflichtenheft, Architekturupdate, Roadmap und README**  
Stand: 18.05.2026

> Diese Featuremap ist keine neue Spezifikation neben den anderen Dokumenten, sondern eine verdichtete Landkarte: Welche Features existieren, welche gehören zu V1, welche zu V1.x/V2/V3 und welche sind bewusst kein Ziel.

## 1. Management Summary

Der SASD Desktop Secret Manager ist als lokaler Windows-Secret-Manager mit mehreren unabhängigen verschlüsselten Tresoren geplant. Der aktuelle Prototyp deckt bereits wichtige V0.x-Bausteine ab: WinForms-Shell, `.svault`, Gruppen, Tags, Suche, CRUD, Drag & Drop und Tests. Für V1 fehlen vor allem Clipboard-Schutz, Datenverlustschutz, Lock/Auto-Lock, Passwortgenerator, Zertifikatseinträge, erweiterte Suche/Filter, Backup-Härtung, Password-Safe-Import und Release-Härtung.

Die wichtigste Änderung gegenüber der älteren Featuremap ist die Aufnahme der HTTPS-/TLS-Zertifikatsverwaltung. Zertifikate werden als eigener Secret-Kontext geführt: V1 erfasst Zertifikatsmetadaten und sensible Key-/PFX-Informationen strukturiert. Ablaufwarnungen folgen in V1.x, verschlüsselte Zertifikatsanhänge in V2 und ACME-/PKI-/Deployment-Automation frühestens in V3/später.

## 2. Status- und Versionslegende

| Status | Bedeutung |
|---|---|
| Umgesetzt (V0.x) | Im Prototyp bereits vorhanden oder wesentlich vorhanden; trotzdem vor V1 nochmals testen. |
| Begonnen | Konzeptionell oder teilweise umgesetzt, aber noch nicht stabil abgeschlossen. |
| V1 Muss | Erforderlich für die runde lokale Erstversion. |
| V1.x Soll | Härtung, Produktpolitur oder praxistauglicher Folgeausbau nach V1. |
| V2 Geplant | Deutlicher Funktionsausbau nach stabiler lokaler Basis. |
| V3/später | Heikle, aufwändige oder strategisch spätere Erweiterung. |
| Nicht-Ziel | Bewusst nicht verfolgt, solange keine neue Entscheidung getroffen wird. |

## 3. Versionssicht

| Version | Ziel | Enthält | Nicht enthalten |
|---|---|---|---|
| V0.x | Prototyp/Entwicklungsstand | .svault, WinForms-Shell, Gruppen, Tags, Suche, CRUD, Drag & Drop, Tests | Nicht produktiv freigeben |
| V1.0 | Runde lokale Erstversion | Clipboard-Schutz, Datenverlustschutz, Auto-Lock, Generator, Zertifikatseinträge, Suche/Filter, Backup-Härtung, .psafe3-Import, Release-Härtung | Keine Cloud, kein Team, kein Mobile, kein ACME/PKI |
| V1.x | Härtung und Produktpolitur | Master-Passwort ändern, Generatorprofile, Tag-Verwaltung, Zertifikatswarnungen, Restore-Komfort, Integritätsprüfung, .psafe3-Export-Vorbereitung | Kein Architekturbruch |
| V2 | Funktionsausbau | Cross-Vault, Historie, TOTP, Anhänge, Mehrsprachigkeit, Themes, optionale Online-Funktionen | Keine Pflicht zur Cloud |
| V3/später | Optionale/heikle Erweiterungen | Browser-Import, Passkeys/WebAuthn-Konzept, ACME/PKI/Deployment, Team/Cloud/Mobile, Recovery ohne Backdoor | Nur nach separatem Konzept |

## 4. Featuremap nach Themen

### A. Fundament, Repository und Architektur

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-001 | C#/.NET-8-WinForms-Desktopanwendung | Umgesetzt (V0.x) | Lokale Windows-Anwendung als primärer Client; bewusst kein Web- oder Cloud-Produkt. |
| FM-002 | Schichtentrennung Domain/Application/Security/Storage/Interop/UI/Tests | Umgesetzt (V0.x) | Fachmodell, Sicherheit, Persistenz und UI bleiben getrennt; neue Features dürfen MainForm nicht überladen. |
| FM-003 | SASD-Dokumentenbasis | Umgesetzt + fortlaufend | Lastenheft, Pflichtenheft, Architektur, Roadmap und README sind führend für die weitere Entwicklung. |
| FM-004 | Entwicklungsstatus sichtbar machen | V1 Muss | README und Doku müssen klar sagen: lauffähiger Prototyp, aber noch kein produktives Sicherheitsprodukt. |
| FM-005 | Repository-Strategie persönlich -> SASD-GmbH Release | V1.x Soll | Entwicklung kann persönlich bleiben; bereinigte Release-Stände können später in SASD-Repos gespiegelt werden. |

### B. Tresorverwaltung, Speicherformat und Dateikontext

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-010 | Mehrere unabhängige Tresore | Umgesetzt/V1 absichern | Private, SASD-interne, kundenbezogene, providerbezogene und Archiv-Tresore bleiben getrennte Dateien mit eigenem Master-Passwort. |
| FM-011 | Neuen Tresor anlegen, öffnen, speichern, speichern unter | Umgesetzt (V0.x) | Grundlegender Vault-Lifecycle ist vorhanden und bleibt V1-Kern. |
| FM-012 | Eigenes .svault-Primärformat | Umgesetzt (V0.x) | Führendes internes Format mit verschlüsselter Nutzlast; Password Safe bleibt Interop, nicht Primärformat. |
| FM-013 | Atomisches Speichern | Umgesetzt/V1 härten | Schreiben über temporäre Datei und kontrollierten Austausch; Fehlerpfade müssen vor V1 sauber geprüft werden. |
| FM-014 | Dateikonflikt-/Zweitöffnungsbehandlung | V1 Muss | Parallele schreibende Bearbeitung desselben Tresors konservativ verhindern oder eindeutig melden. |
| FM-015 | Formatversion und Migrationsfähigkeit | V1 Muss | Neue EntryTypes und CustomFieldKinds dürfen alte Tresore nicht beschädigen; unbekannte Felder nicht still verlieren. |
| FM-016 | MRU-Liste zuletzt verwendeter Tresore | V1.x Soll | Komfortfunktion ohne Speicherung von Master-Passwörtern oder geheimen Tresorinhalten. |

### C. Fachliches Datenmodell und allgemeine Secret-Typen

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-020 | SecretEntry als zentrale fachliche Einheit | Umgesetzt (V0.x) | Ein Eintrag beschreibt einen primären Zugang oder ein primäres Secret, nicht eine beliebige Sammelnotiz. |
| FM-021 | Gruppen, Untergruppen und Tags | Umgesetzt (V0.x) | Feste Ablage über Gruppen, flexible Querbezüge über Tags. |
| FM-022 | CustomFields mit Secret-Kennzeichnung | Umgesetzt/V1 prüfen | Zusatzfelder bilden Host, Port, Endpunkte, Rollen, Zertifikatsdaten und geheime Werte strukturiert ab. |
| FM-023 | Eintragstypen Login, Mail, Database, Hosting, FTP/SFTP, API, Server, License, SecureNote, Custom | V1 Muss | Typen steuern UI-Vorschläge, Filter und spätere Templates, ohne das flexible Modell zu zerstören. |
| FM-024 | Zeitstempel CreatedUtc/ModifiedUtc | Umgesetzt (V0.x) | Grundlage für Nachvollziehbarkeit, Sortierung, spätere Historie und mögliche Konfliktanalyse. |
| FM-025 | Gruppenweite Default-Passwörter | Nicht-Ziel | Bewusst verworfen; ersetzt durch Generator, Profile, Templates und Default-Metadaten. |
| FM-026 | Eintragsvorlagen allgemein | V1.x Soll | Vorlagen für häufige Typen vereinfachen Erfassung, bleiben aber auf dem flexiblen CustomField-Modell. |

### D. HTTPS-/TLS-Zertifikatsverwaltung

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-040 | EntryType HttpsTlsCertificate | V1 Muss | Zertifikate sind eigener Secret-Kontext und dürfen nicht nur als freie Notiz versteckt werden. |
| FM-041 | Zertifikatsidentität erfassen | V1 Muss | Common Name/Domain, Subject, SANs, Issuer, Seriennummer und Fingerprint strukturiert speichern. |
| FM-042 | Gültigkeitsdaten erfassen | V1 Muss | ValidFrom und ValidTo als Datumsfelder erfassen und in Detailansicht sichtbar machen. |
| FM-043 | Deployment-/Nutzungskontext erfassen | V1 Muss | DeploymentTarget, Server/Host, Anwendung, Provider, Umgebung und Verantwortlichkeit als strukturierte Felder. |
| FM-044 | Zertifikatstext PEM/CRT speichern | V1 Muss | Öffentlicher Zertifikatstext ist nicht automatisch geheim, aber kontrolliert darzustellen und nicht unnötig in Listen zu zeigen. |
| FM-045 | Private Key / PFX / P12 sensibel behandeln | V1 Muss | PrivateKeyPem, Pfx/P12-Referenz, PFX-Passwort und Private-Key-Passphrase sind besonders sensible Secret-Felder. |
| FM-046 | Zertifikats-Detailansicht | V1 Muss | Domain, SANs, Issuer, Fingerprint, ValidTo und Deployment klar gruppiert anzeigen; Key-Bereich standardmäßig maskiert. |
| FM-047 | Zertifikatssuche | V1 Muss | Suche über Domain, SANs, Issuer, Fingerprint und DeploymentTarget; geheime Key-Werte nicht offenlegen. |
| FM-048 | Zertifikatsablauf-Warnungen | V1.x Soll | Warnschwellen z. B. 30/14/7 Tage; Übersicht bald ablaufender Zertifikate ohne automatische Erneuerungszusage. |
| FM-049 | Zertifikatsanhänge | V2 Geplant | CRT/CER/PEM/PFX/Chain-Dateien verschlüsselt als Anhänge im Tresor; keine Klartext-Nebenablage. |
| FM-050 | ACME-/PKI-/Deployment-Automation | V3/später | Kein V1-Ziel; erfordert eigenes Sicherheits-, Betriebs- und Haftungskonzept. |

### E. Eintragsverwaltung und Detailinteraktion

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-060 | Einträge anlegen, bearbeiten, löschen | Umgesetzt (V0.x) | CRUD für Secrets ist vorhanden und bleibt Kern der UI. |
| FM-061 | Reveal/Hide geheimer Werte | Umgesetzt/V1 härten | Secrets standardmäßig maskiert; Offenlegung nur bewusst und temporär. |
| FM-062 | Copy-Aktionen für Secret und Zusatzfelder | Begonnen/V1 Muss | Alle sensiblen Copy-Pfade müssen über zentrale Clipboard-Schutzlogik laufen. |
| FM-063 | Typbezogene Feldgruppen | V1 Muss | Zertifikate, Datenbanken, Mail, API und FTP/SFTP sollen verständliche Feldgruppen statt chaotischer Freitextlisten erhalten. |
| FM-064 | Validierung wichtiger Eingaben | V1 Muss | Titel, Typ, Datumsfelder, Portwerte und sensible Pflichtfelder plausibel prüfen, ohne Nutzer zu stark einzuschränken. |
| FM-065 | Archivieren statt Löschen | V2 Geplant | Optionaler späterer Status, damit alte Einträge verschwinden können, ohne sofort gelöscht zu werden. |

### F. Organisation, Navigation, Suche und Filter

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-080 | Root-/Tresorknoten sichtbar | Umgesetzt (V0.x) | Hauptgruppen können direkt am Tresor entstehen; Root-Ansicht bleibt ruhig und verständlich. |
| FM-081 | Kontextmenüs für Gruppen und Einträge | Umgesetzt (V0.x) | Wichtige Aktionen über Kontextmenüs statt überladener Button-Leisten. |
| FM-082 | Drag & Drop für Einträge | Umgesetzt (V0.x) | Einträge zwischen Gruppen verschieben; ungültige Ziele blockieren. |
| FM-083 | Drag & Drop für Gruppen | Umgesetzt (V0.x) | Gruppen verschieben mit Schutz gegen Zyklen, Drop auf sich selbst und riskante Hierarchiefehler. |
| FM-084 | Freitextsuche case-insensitive | Umgesetzt/V1 erweitern | Suche über Titel, Nutzer, Tags, Gruppen, Notizen und nicht-geheime Zusatzfelder ausbauen. |
| FM-085 | Typfilter und Tagfilter | V1 Muss | Alltagstaugliches Wiederfinden nach Secret-Typ und Tags; besonders wichtig bei Zertifikaten und technischen Secrets. |
| FM-086 | Tag-Verwaltung | V1.x Soll | Tags umbenennen, zusammenführen, bereinigen und unbenutzte Tags entfernen. |
| FM-087 | Globale Multi-Vault-Suche | V2 Geplant | Erst nach stabiler Trennung der Tresore; keine unkontrollierte Entsperrung mehrerer Vaults. |

### G. Laufzeitsicherheit und Bedien-Schutz

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-100 | Master-Passwort pro Tresor | Umgesetzt/V1 härten | Master-Passwort schützt den Vault und darf nie geloggt oder außerhalb des Entsperrkontexts gespeichert werden. |
| FM-101 | Passwortstärke-Bewertung | Umgesetzt (V0.x) | Warnung bei sehr schwachen Master-Passwörtern; keine falsche Garantie für perfekte Sicherheit. |
| FM-102 | PBKDF2-SHA256 + AES-256-GCM im aktuellen Format | Umgesetzt (V0.x) | Aktuelle konsistente Linie des Prototyps; KDF-Parameter bleiben versions- und migrationsfähig. |
| FM-103 | Zentrale Clipboard-Autoclear-Logik | Begonnen/V1 Muss | Sensible kopierte Werte nach konfigurierbarer Zeit entfernen; Statusfeedback geben. |
| FM-104 | Manuelles Sperren und Auto-Lock | V1 Muss | Tresor nach Inaktivität sperren, Details maskieren, erneute Entsperrung mit Master-Passwort. |
| FM-105 | Keine Secrets in Logs | V1 Muss | Logs, Exceptions, Importberichte und Debug-Ausgaben müssen Redaction-Regeln befolgen. |
| FM-106 | Master-Passwort ändern/Re-Encryption | V1.x Soll | Altes Passwort validieren, mit neuem Passwort neu verschlüsseln, Backup/Fehlerfall absichern. |
| FM-107 | Recovery ohne Backdoor | V3/später | Nur als explizites Recovery-Verfahren; keine versteckte Hersteller- oder Entwicklerhintertür. |
| FM-108 | Virtuelle Tastatur | Nicht-Ziel | Sicherheitsgewinn fraglich, Bedienaufwand hoch; aktuell nicht verfolgen. |

### H. Backup, Restore, Datenverlustschutz und Integrität

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-120 | Dirty-State über alle Änderungen | V1 Muss | Eintrags-, Gruppen-, Tag-, Drag-and-Drop- und Zertifikatsänderungen setzen Dirty konsistent. |
| FM-121 | Rückfragen bei Neu/Öffnen/Beenden | V1 Muss | Ungespeicherte Änderungen nicht still verlieren. |
| FM-122 | Verschlüsselte Backup-Dateien | Umgesetzt/V1 härten | Backups müssen denselben Schutzanspruch wie Primärdateien erfüllen. |
| FM-123 | Zusätzliche LocalAppData-Sicherung | V1.x Soll | Zweite Rückfallebene, ebenfalls verschlüsselt, ohne geheime Klartextdateien. |
| FM-124 | Restore-Komfort | V1.x Soll | Backup-Kette sichtbar machen und Wiederherstellung sicher führen. |
| FM-125 | Integritätsprüfung eigener Programmdateien | V1.x Soll | Hash-/Manifestprüfung als begrenzte Zusatzmaßnahme, nicht als Malware-Schutzversprechen. |

### I. Interoperabilität, Import und Export

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-140 | Password-Safe-Import .psafe3 | V1 Muss | Import-Assistent mit Mapping und Bericht; interne Datenstruktur bleibt führend. |
| FM-141 | Importbericht und Verlusthinweise | V1 Muss | Nicht direkt abbildbare Inhalte sichtbar markieren, nicht still verschlucken. |
| FM-142 | Password-Safe-Export | V1.x/V2 Geplant | Nur für klar abbildbare Daten mit Kompatibilitätsbericht; Zertifikatseinträge voraussichtlich verlustbehaftet. |
| FM-143 | CSV-Export | V2 Geplant | Nur nach expliziter Warnung, weil CSV unverschlüsselte hochkritische Exportdateien erzeugen kann. |
| FM-144 | Browser-Import | V3/später | Heikel wegen Datenschutz, Plattformabhängigkeit und sensiblen Browser-Speichern; nicht V1. |
| FM-145 | Import aus KeePass/Bitwarden/1Password etc. | V3/später/offen | Erst nach stabiler V1 und klaren Mapping-Regeln; nicht in den frühen Umfang ziehen. |

### J. Komfort, Produktpräsenz und spätere Erweiterungen

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-160 | Eigenes Icon und konsistentes Branding | V1.x Soll | Produkt wirkt professioneller; kann aber V1-Funktionalität nicht ersetzen. |
| FM-161 | Mehrsprachigkeit DE/EN | V2 Geplant | Ressourcenmechanismus vorbereiten, aber nicht vor V1 erzwingen. |
| FM-162 | Hell-/Dunkel-Theme | V2 Geplant | Nützlich für Präsentation und Alltag, aber kein Sicherheitskern. |
| FM-163 | TOTP-Verwaltung | V2 Geplant | Komfortabel, aber Faktortrennung beachten, wenn Passwort und TOTP im selben Tresor liegen. |
| FM-164 | Passwort-/Secret-Historie | V2 Geplant | Fachlich nützlich, aber alte Secrets erhöhen Risiko und brauchen klare Begrenzung. |
| FM-165 | Passkeys/WebAuthn | V3/später/offen | Separates Konzept nötig; nicht wie klassische Passwörter modellieren. |
| FM-166 | Update-Suche, News, Leak-Prüfung | V2/V3 Opt-in | Nur optional, transparent und datenschutzbewusst; keine Online-Pflicht. |
| FM-167 | Team/Cloud/Mobile | V3/später/offen | Erst nach stabilem lokalem Kern; betrifft Rollen, Offboarding, Sync, Datenschutz und Betrieb. |

### K. Tests, Qualitätssicherung und Abnahme

| ID | Feature | Status/Version | Beschreibung |
|---|---|---|---|
| FM-180 | Unit-Tests für Domain/Application/Security/Storage/Interop | Umgesetzt + fortlaufend | Vorhandene Tests ausbauen; neue Zertifikats- und Clipboard-Fälle ergänzen. |
| FM-181 | V1-End-to-End-Testkatalog | V1 Muss | Manuelle und automatisierte Abnahmepfade für Anlegen, Speichern, Öffnen, Lock, Clipboard, Zertifikate, Import. |
| FM-182 | Security-Review vor V1 | V1 Muss | Logs, Clipboard, UI-Reveal, Export/Import, Zertifikatsfelder und Backup-Pfade prüfen. |
| FM-183 | Dokumentation synchron halten | V1 Muss | README, Lastenheft, Pflichtenheft, Architektur, Roadmap und Featuremap verwenden dieselben Begriffe. |
| FM-184 | Release-Notizen und Benutzerkurzdoku | V1 Muss | Vor ernsthafter Nutzung klar beschreiben, was funktioniert, was nicht und welche Sicherheitsgrenzen gelten. |

## 5. Zertifikats-Feldkonventionen

| Feld | Art | Geheim? | Zweck |
|---|---|---|---|
| Domain / CommonName | Text/Hostname | Nein | Primäre Domain oder Zertifikatsname; Listen- und Suchfeld. |
| SubjectAlternativeNames | MultilineText/Textliste | Nein | Mehrere DNS-/IP-SANs; durchsuchbar und in Detailansicht gruppiert. |
| Issuer | Text | Nein | Aussteller/CA für Einordnung und Suche. |
| SerialNumber | Text | Nein | Eindeutige Seriennummer; technische Identifikation. |
| FingerprintSha256 | Text | Nein | Prüfwert; gut kopierbar, aber nicht als Secret behandeln. |
| ValidFrom / ValidTo | Date | Nein | Gültigkeitszeitraum; Basis für spätere Ablaufwarnungen. |
| CertificatePem / CertificateText | CertificateText/MultilineText | Normalerweise nein | Öffentlicher Zertifikatstext; kontrolliert anzeigen, aber nicht wie Private Key behandeln. |
| PrivateKeyPem | Secret/MultilineSecret | Ja, hochkritisch | Privater Schlüssel; maskieren, Clipboard-Schutz, keine Klartext-Exports ohne ausdrückliche Warnung. |
| PrivateKeyPassphrase | Secret | Ja | Passphrase für privaten Schlüssel; wie Passwort behandeln. |
| PfxPassword | Secret | Ja | Passwort für PFX/P12; wie Passwort behandeln. |
| DeploymentTarget | Text/Hostname/URL | Nein | Server, Anwendung oder Webspace, auf dem das Zertifikat verwendet wird. |
| RenewalHint / Provider | Text | Nein | Hinweis auf Erneuerung, CA, ACME-Account oder Provider; V1 nur manuell dokumentieren. |

## 6. Feature-Abhängigkeiten

| Abhängigkeit | Kette | Grund |
|---|---|---|
| Clipboard-Schutz vor Zertifikat-Key-Feldern | FM-103 -> FM-045/FM-046 | Private Keys dürfen erst sauber kopierbar sein, wenn Copy-Pfade zentral geschützt sind. |
| Datenmodell vor Zertifikats-UI | FM-015/FM-040 -> FM-046 | Zertifikatsdialoge brauchen stabile EntryTypes und Feldkonventionen. |
| Dirty-State vor Release | FM-120/FM-121 -> FM-184 | V1 darf keine stillen Datenverluste erzeugen. |
| Backup-Härtung vor Master-Passwort-Wechsel | FM-122/FM-124 -> FM-106 | Re-Encryption ist riskant, wenn Backup- und Fehlerpfade nicht stabil sind. |
| Importbericht vor Fremdformat-Export | FM-141 -> FM-142/FM-143 | Erst lesen und erklären, dann kontrolliert exportieren. |
| Lokaler Kern vor Team/Cloud/Mobile | V1 -> FM-167 | Sync, Rollen und Offboarding nur nach stabilem Einzelplatzprodukt. |

## 7. Nächste Arbeitspakete

| Reihenfolge | Arbeitspaket | Features | Ziel |
|---|---|---|---|
| 1 | Clipboard-Schutz fertigstellen | FM-103, FM-062 | Milestone 12 sauber abschließen; Copy-Pfade zentralisieren. |
| 2 | Dirty-State und Datenverlustschutz stabilisieren | FM-120, FM-121 | Alle Mutation- und Abbruchpfade prüfen. |
| 3 | Lock/Auto-Lock implementieren | FM-104 | Secret-UI beim Sperren leeren/maskieren. |
| 4 | Passwortgenerator-Basis bauen | FM-025, FM-026 | Generator statt unsicherer Default-Passwörter. |
| 5 | Datenmodell für Zertifikate ergänzen | FM-040 bis FM-045 | EntryType und Feldkonventionen vor UI-Feldgruppen. |
| 6 | Zertifikats-UI und Suche ergänzen | FM-046, FM-047 | Detail-/Editierbereich und Suchabdeckung. |
| 7 | Backup-/Import-/Release-Härtung abschließen | FM-122, FM-140, FM-184 | V1-Release vorbereiten. |

## 8. Bewusste Nicht-Ziele und Warnhinweise

- Kein 1:1-Klon von Password Safe. Das interne Modell bleibt führend; Password Safe ist Interop.
- Keine Recovery-Backdoor. Recovery darf später nur als explizites, dokumentiertes Verfahren entstehen.
- Keine ACME-/PKI-/Server-Deployment-Automation in V1.
- Keine Cloud-, Team-, Mobile- oder Browser-Autofill-Pflicht vor stabiler lokaler V1.
- Keine Klartext-Exportdateien ohne deutliche Warnung und bewusste Nutzerentscheidung.
- Keine geheimen Werte in Logs, Debug-Ausgaben, Fehlerdialogen, Listenansichten oder unverschlüsselten Temporärdateien.

## 9. Pflegehinweis

Diese Featuremap sollte nach jedem größeren Milestone aktualisiert werden. Neue Features werden nicht sofort in V1 aufgenommen, sondern zuerst einer Version, einem Risiko und einem Abnahmekriterium zugeordnet. Für sicherheitsrelevante Features gilt: erst Schutzmodell und Tests, dann UI-Komfort.