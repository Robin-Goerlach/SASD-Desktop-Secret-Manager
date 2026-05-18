# Test- und Abnahmekatalog V1: SASD Desktop Secret Manager

**Stand:** 18.05.2026  
**Status:** Arbeits- und Freigabegrundlage für V1  
**Bezug:** README, Lastenheft, Pflichtenheft, Architekturupdate, Featuremap, Roadmap und Security-/Threat-Model


# Management Summary

Der SASD Desktop Secret Manager verwaltet sicherheitskritische Daten. Für V1 reicht es daher nicht, dass die Anwendung startet und typische Bedienwege funktionieren. V1 ist erst abnahmefähig, wenn die Kernfunktionen fachlich vollständig, sicherheitsbewusst, reproduzierbar getestet und dokumentiert sind.

Dieser Test- und Abnahmekatalog übersetzt Lastenheft, Pflichtenheft, Architektur, Roadmap, Featuremap und Security-/Threat-Model in prüfbare Testfälle und Release-Gates. Er enthält automatisierte Testziele, manuelle Abnahmeszenarien, Negativtests, Sicherheitsprüfungen, Dokumentationsprüfungen und eine klare Entscheidung, wann V1 als rund genug gilt.

Besonders berücksichtigt werden die nachträglich geschärften Anforderungen: HTTPS/TLS-Zertifikatsverwaltung, sichere Behandlung von Private-Key-/PFX-/Passphrase-Feldern, Clipboard-Autoclear, Auto-Lock, Datenverlustschutz, Backup-Härtung, .svault-Dateiformat, Password-Safe-Import und die klare Abgrenzung gegen Cloud-, Team-, Browser- und ACME-/PKI-Automation in V1.


# Ziel, Geltungsbereich und Abgrenzung

Das Dokument gilt für die geplante Version V1.0 des SASD Desktop Secret Managers. Es beschreibt, welche Prüfungen vor einer ernsthaften V1-Freigabe durchgeführt werden müssen und welche Ergebnisse als bestanden gelten.

V1 bleibt eine lokale Windows-Desktop-Anwendung für Einzelplatznutzung. Mehrere unabhängige Tresore, verschlüsseltes Speichern und Laden, Gruppen, Tags, Suche, strukturierte technische Secret-Typen, HTTPS/TLS-Zertifikate, Clipboard-Schutz, Auto-Lock, Passwortgenerator, Backup-Härtung und Password-Safe-Import bilden den relevanten Prüfumfang.

Nicht Gegenstand der V1-Abnahme sind Cloud-Synchronisation, Team-Sharing, Mobile-Clients, Browser-Autofill, ACME-/PKI-Automation, automatische Zertifikatserneuerung, echte Enterprise-Rechteverwaltung, Passkeys/WebAuthn, TOTP als verpflichtender V1-Baustein und ein Recovery-Verfahren ohne Backdoor. Diese Punkte dürfen die V1-Freigabe nicht blockieren, müssen aber als bewusst nicht enthalten dokumentiert sein.


# Grundsätze der V1-Abnahme

V1 wird nicht nach Bauchgefühl freigegeben. Die Abnahme folgt vier Grundsätzen:

1. Sicherheitsrelevante Fehler schlagen Komfortfortschritte. Ein schönes UI kann keine Secret-Leaks, defekte Backups oder unsichere Exportpfade ausgleichen.
2. Muss-Funktionen benötigen konkrete Tests. Eine Funktion gilt nicht als erledigt, wenn sie nur im Code existiert, sondern erst, wenn positive, negative und relevante Randfälle geprüft sind.
3. Keine echten Secrets in Tests. Testdaten bestehen aus künstlichen Passwörtern, Dummy-Domains, selbst erzeugten Testzertifikaten und absichtlich unbrauchbaren Beispiel-Keys.
4. Dokumentation ist Teil der Abnahme. README, Featuremap, Roadmap, Pflichtenheft, Architektur und Security-Dokument müssen dieselben Begriffe und Versionsgrenzen verwenden.


# Teststrategie und Testebenen

Die Teststrategie kombiniert automatisierte Tests und manuelle Abnahmeläufe. Automatisierte Tests prüfen Domain-, Application-, Security-, Storage- und Interop-Logik. Manuelle Tests prüfen UI-Flows, Dialogverhalten, Benutzerführung, Statusmeldungen und sichtbare Sicherheitsgrenzen.

Besonders wichtig ist die Trennung von UI und Fachlogik. Sicherheits- und Dateiformatfunktionen dürfen nicht nur durch manuelle Klicktests abgesichert werden. UI-Tests sind dort nötig, wo die tatsächliche Bedienung geprüft wird; die zugrunde liegende Policy muss zusätzlich isoliert testbar sein.


# Qualitäts- und Fehlerschwereklassen

Die folgende Einstufung entscheidet, ob ein Fehler die V1-Freigabe blockiert. Besonders bei einem Secret Manager sind Security- und Datenverlustfehler strenger zu behandeln als reine Komfortprobleme.


| Stufe | Bedeutung | V1-Freigaberegel |
| --- | --- | --- |
| S0 Blocker | Datenverlust, Secret-Leak, nicht öffnbarer Tresor, reproduzierbarer Crash in Kernfunktion. | V1 darf nicht freigegeben werden. |
| S1 Kritisch | Sicherheitsgrenze verletzt, Import verschluckt Daten, Backup/Restore unklar, Auto-Lock/Clipboard fehlerhaft. | Vor V1 beheben oder Funktion bewusst deaktivieren. |
| S2 Hoch | Wichtige V1-Funktion unvollständig, irreführende UI, fehlende Tests für Kernpfad. | Vor V1 beheben oder mit dokumentierter Einschränkung entscheiden. |
| S3 Mittel | Komfortproblem, kleine Inkonsistenz, nicht sicherheitskritischer UI-Fehler. | Darf nur mit Ticket und Workaround in V1 bleiben. |
| S4 Niedrig | Kosmetik, Text, Layout, kleinere Politur. | Darf in V1 bleiben, wenn dokumentiert. |


# Release-Gates für V1

Die folgenden Gates müssen vor einer V1-Freigabe bewusst geprüft werden. Ein Muss-Gate darf nicht ignoriert werden. Wenn ein Gate nicht erfüllt ist, wird V1 nicht freigegeben oder die betroffene Funktion bleibt ausdrücklich deaktiviert und dokumentiert.


| RG-01 | Repository- und Build-Gate | dotnet restore, dotnet build und dotnet test laufen ohne kritische Fehler durch. | Muss |
| --- | --- | --- | --- |
| RG-02 | Testdaten-Gate | Keine echten produktiven Passwörter, Tokens, Private Keys, Zertifikate oder Kundendaten im Repository oder in Testdateien. | Muss |
| RG-03 | Vault-Lifecycle-Gate | Neuer Tresor kann erstellt, gespeichert, geschlossen, wieder geöffnet und weiterbearbeitet werden. | Muss |
| RG-04 | Verschlüsselungs- und Storage-Gate | .svault-Dateien sind nicht als Klartext lesbar; falsches Passwort und beschädigte Datei werden sauber behandelt. | Muss |
| RG-05 | Datenverlustschutz-Gate | Ungespeicherte Änderungen werden bei Neu, Öffnen, Beenden und Schließen nicht still verworfen. | Muss |
| RG-06 | Clipboard-Gate | Sensible Copy-Aktionen laufen über zentrale Schutzlogik und werden automatisch geleert. | Muss |
| RG-07 | Lock-Gate | Manuelles Sperren und Auto-Lock verbergen sensible Inhalte und erfordern erneute Entsperrung. | Muss |
| RG-08 | Zertifikats-Gate | HTTPS/TLS-Zertifikatseinträge mit privaten Schlüsselfeldern werden korrekt gespeichert, angezeigt, maskiert, gesucht und kopiert. | Muss |
| RG-09 | Backup-/Restore-Gate | Backups bleiben verschlüsselt; Fehlerfälle und Konflikte werden nachvollziehbar gemeldet. | Muss |
| RG-10 | Password-Safe-Import-Gate | .psafe3-Import erzeugt nachvollziehbares Mapping und Importbericht ohne stillen Datenverlust. | Muss |
| RG-11 | Log-/Redaction-Gate | Keine Master-Passwörter, Secrets, Tokens, PFX-Passwörter oder Private Keys in Logs, Fehlerdialogen oder Debug-Ausgaben. | Muss |
| RG-12 | Dokumentations-Gate | README, Benutzerhinweise, Security-Hinweise und Known Limitations passen zum tatsächlichen V1-Stand. | Muss |


# Abdeckungsmatrix

Die Matrix verbindet Funktionsbereiche mit Testarten und Risiken. Sie verhindert, dass neue Themen wie HTTPS/TLS-Zertifikate nur dokumentiert, aber nicht abgenommen werden.


| Bereich | V1-Relevanz | Automatisiert | Manuell | Wichtigste Risiken |
| --- | --- | --- | --- | --- |
| Projektstruktur/Build | Muss | Ja | Kurz | Nicht reproduzierbarer Stand, fehlende Tests. |
| Vault Lifecycle | Muss | Ja | Ja | Datenverlust, Passwortfehler, nicht ladbarer Tresor. |
| .svault und Verschlüsselung | Muss | Ja | Teilweise | Klartext, defekte Migration, beschädigte Datei. |
| Mehrtresor-Trennung | Muss | Ja | Ja | Vermischung privater, SASD- und Kundendaten. |
| Einträge/CustomFields | Muss | Ja | Ja | Unvollständige technische Secrets, verlorene Zusatzfelder. |
| HTTPS/TLS-Zertifikate | Muss | Ja | Ja | Private-Key-Leak, falsche Ablauf-/Domain-Daten. |
| Gruppen/Tags/Drag-and-Drop | Muss | Ja | Ja | Zyklen, falsche Zuordnung, stiller Datenverlust. |
| Suche/Filter | Muss | Ja | Ja | Einträge nicht auffindbar, geheime Felder falsch behandelt. |
| Clipboard/Reveal | Muss | Ja | Ja | Secrets bleiben zu lange sichtbar oder in Zwischenablage. |
| Lock/Auto-Lock | Muss | Ja | Ja | Offener Tresor bleibt sichtbar bei Abwesenheit. |
| Passwortgenerator | Muss | Ja | Ja | Schwache Defaults, verwirrende Optionen. |
| Backups/Konflikte | Muss | Ja | Ja | Unverschlüsselte Backups, defekter Restore, Parallelbearbeitung. |
| Password-Safe-Import | Muss | Ja | Ja | Stiller Datenverlust, falsches Mapping, unklare Warnungen. |
| Logging/Fehlerbehandlung | Muss | Ja | Ja | Secrets im Log, technische Fehlermeldung zu kryptisch. |
| Dokumentation/Release | Muss | Nein | Ja | README verspricht mehr als V1 liefert. |


# Detail-Testfälle

Die folgenden Testfälle bilden den ausführlichen Katalog für V1. Sie können später direkt in GitHub Issues, Checklisten oder Testmanagement-Tabellen überführt werden. Jeder Testfall sollte ein Ergebnis erhalten: bestanden, fehlgeschlagen, blockiert oder nicht anwendbar.


| ID | Titel | Bereich | Priorität | Art | Vorbedingung | Schritte | Erwartetes Ergebnis |
| --- | --- | --- | --- | --- | --- | --- | --- |
| TC-BLD-001 | Restore, Build und Test laufen grün | Build/CI | Muss | Automatisiert/Manuell | Repository frisch ausgecheckt; .NET 8 SDK vorhanden. | dotnet restore, dotnet build, dotnet test ausführen. | Alle Befehle laufen ohne Fehler; keine kritischen Warnungen, die Security oder Datenverlust betreffen. |
| TC-BLD-002 | WinForms-Anwendung startet ohne Demo- oder Debug-Crash | Build/Run | Muss | Manuell | Build erfolgreich. | dotnet run --project src/Sasd.SecretManager.WinForms/Sasd.SecretManager.WinForms.csproj ausführen. | Hauptfenster öffnet; Debug-Logging startet kontrolliert; keine Ausnahme beim Start. |
| TC-BLD-003 | Projektstruktur entspricht Schichtenmodell | Architektur | Muss | Review | Repository liegt vor. | Prüfen, ob Domain, Application, Security, Storage, Interop.PasswordSafe, WinForms und Tests getrennt bleiben. | Keine Kryptografie in UI-Forms; keine UI-Abhängigkeiten in Domain; neue Zertifikatslogik nicht monolithisch in MainForm versteckt. |
| TC-VLT-001 | Neuen Tresor anlegen und speichern | Vault Lifecycle | Muss | Manuell/Integration | Anwendung gestartet; keine echten Secrets verwenden. | Neuen Tresor mit Dummy-Master-Passwort anlegen, Beispielgruppe und Eintrag erstellen, als .svault speichern. | Datei wird erzeugt; UI zeigt gespeicherten Zustand; keine Klartextdaten außerhalb der Datei. |
| TC-VLT-002 | Gespeicherten Tresor erneut öffnen | Vault Lifecycle | Muss | Manuell/Integration | TC-VLT-001 abgeschlossen. | Anwendung schließen, neu starten, .svault öffnen, korrektes Master-Passwort eingeben. | Tresor lädt vollständig; Gruppen, Tags, Einträge und Zusatzfelder sind unverändert vorhanden. |
| TC-VLT-003 | Falsches Master-Passwort ablehnen | Vault Lifecycle | Muss | Integration/Manuell | Ein gültiger Testtresor existiert. | Tresor öffnen und bewusst falsches Master-Passwort eingeben. | Tresor wird nicht geöffnet; Fehlermeldung ist verständlich und enthält keine Details, die Angreifern helfen oder Secrets offenlegen. |
| TC-VLT-004 | Mehrere Tresore getrennt halten | Mehrtresor | Muss | Manuell/Integration | Zwei Testtresore existieren: Privat-Test und SASD-Test. | Beide nacheinander öffnen; unterschiedliche Einträge anlegen; speichern; erneut öffnen. | Einträge vermischen sich nicht; jeder Tresor verlangt sein eigenes Master-Passwort; MRU enthält keine Secrets. |
| TC-VLT-005 | Speichern unter erzeugt getrennte Datei | Vault Lifecycle | Muss | Manuell | Ein Testtresor ist geöffnet. | Speichern unter neuem Dateinamen ausführen; beide Dateien nacheinander öffnen. | Beide Tresore sind unabhängig lesbar; Änderungen an einer Datei wirken nicht ungewollt auf die andere. |
| TC-STO-001 | .svault-Datei enthält keinen Klartext-Payload | Storage/Security | Muss | Automatisiert/Review | Testtresor mit markanten Dummy-Werten existiert. | Datei mit Texteditor/Strings prüfen; nach Dummy-Passwort, Domain, Token und Private-Key-Testwert suchen. | Keine sensiblen Dummy-Werte sind im Klartext auffindbar; Header enthält nur notwendige nicht-geheime Metadaten. |
| TC-STO-002 | Beschädigte Datei sauber melden | Storage/Security | Muss | Integration/Manuell | Kopie eines gültigen Testtresors. | Datei abschneiden oder Bytes verändern; öffnen. | Anwendung meldet beschädigte/ungültige Datei; kein Crash; keine Teilrettung als Erfolgsmeldung. |
| TC-STO-003 | Unbekannte spätere Felder führen nicht zu Datenverlust | Storage/Migration | Soll für V1 | Automatisiert | Manipulierter Testpayload bzw. vorbereitete Testdatei. | Tresor mit unbekannten nicht-kritischen Metadaten laden und speichern. | Anwendung verhält sich definiert: entweder Felder bleiben erhalten oder werden mit klarer Migrationsregel behandelt; kein stiller Datenverlust. |
| TC-STO-004 | Backup bleibt verschlüsselt | Backup/Security | Muss | Automatisiert/Manuell | Bestehender Testtresor wird überschrieben. | Änderung speichern, Backup-Datei prüfen. | Backup-Datei ist vorhanden, weiterhin verschlüsselt und enthält keine Klartext-Secrets. |
| TC-STO-005 | Dateikonflikt bei Zweitöffnung konservativ behandeln | Storage/Concurrency | Muss | Manuell | Ein Tresor ist geöffnet. | Versuchen, dieselbe Datei ein zweites Mal schreibend zu öffnen oder parallel zu speichern. | Anwendung verhindert parallele schreibende Bearbeitung oder warnt klar; kein stilles Überschreiben. |
| TC-ENT-001 | Login-Eintrag vollständig bearbeiten | Einträge | Muss | Manuell/Unit | Testtresor geöffnet. | Login mit Titel, Benutzername, Passwort, URL, Notiz, Tags und Gruppe anlegen; speichern; erneut öffnen. | Alle Felder bleiben erhalten; Passwort ist standardmäßig maskiert. |
| TC-ENT-002 | Datenbank-Eintrag mit technischen Zusatzfeldern | Einträge/CustomFields | Muss | Manuell/Unit | Testtresor geöffnet. | Database-Eintrag mit Host, Port, Datenbankname, Schema, Benutzername, Passwort, SSL-Hinweis anlegen. | Zusatzfelder werden strukturiert gespeichert, angezeigt und durchsucht; geheime Felder bleiben maskiert. |
| TC-ENT-003 | Mail-Eintrag mit IMAP/SMTP-Daten | Einträge/CustomFields | Muss | Manuell | Testtresor geöffnet. | Mail-Eintrag mit E-Mail, IMAP-Host/Port, SMTP-Host/Port, TLS-Hinweis und Passwort anlegen. | Felder bleiben erhalten; Suche findet über E-Mail und Host; Passwort bleibt Secret. |
| TC-ENT-004 | API-Secret mit Token und Endpoint | Einträge/CustomFields | Muss | Manuell/Unit | Testtresor geöffnet. | API-Eintrag mit Endpoint, Client-ID, Token/Secret, Scope und Notiz anlegen. | Token wird geheim behandelt; Endpoint und Scope sind durchsuchbar. |
| TC-ENT-005 | SecureNote ohne klassisches Passwort | Einträge | Muss | Manuell | Testtresor geöffnet. | SecureNote mit Notiz und optionalem geheimem Zusatzfeld anlegen. | Eintrag kann ohne klassischen Login genutzt werden; geheime Zusatzfelder bleiben geschützt. |
| TC-ENT-006 | Änderungszeitpunkt wird aktualisiert | Einträge/Audit | Muss | Automatisiert/Manuell | Ein bestehender Eintrag. | Ein Feld ändern und speichern. | ModifiedUtc wird aktualisiert; CreatedUtc bleibt erhalten. |
| TC-ENT-007 | Abbrechen im Editierdialog verändert nichts | Einträge/UI | Muss | Manuell/Unit | Ein bestehender Eintrag. | Eintrag bearbeiten, mehrere Werte ändern, dann Abbrechen. | Kein Feld wird geändert; Dirty-State bleibt unverändert, falls vorher sauber. |
| TC-CERT-001 | HTTPS/TLS-Zertifikatseintrag manuell erfassen | Zertifikate | Muss | Manuell/Unit | Testtresor geöffnet; nur Dummy-/Testzertifikat verwenden. | Zertifikatseintrag mit Domain, SANs, Issuer, SerialNumber, FingerprintSha256, ValidFrom, ValidTo, DeploymentTarget und Notizen anlegen. | Alle nicht-geheimen Zertifikatsdaten werden strukturiert gespeichert und in der Detailansicht sichtbar angezeigt. |
| TC-CERT-002 | Private-Key-Feld als hochkritisches Secret behandeln | Zertifikate/Security | Muss | Manuell/Unit | Zertifikatseintrag existiert. | Dummy PrivateKeyPem oder PrivateKeyPassphrase eintragen, Detailansicht öffnen. | Private-Key-/Passphrase-Felder sind maskiert; Reveal und Copy sind explizit; Clipboard-Schutz greift. |
| TC-CERT-003 | PFX/P12-Passwort schützen | Zertifikate/Security | Muss | Manuell/Unit | Zertifikatseintrag existiert. | PfxPassword eintragen, speichern, neu öffnen, kopieren. | PfxPassword wird wie Passwort behandelt, nicht in Listen oder Logs angezeigt und nach Copy automatisch geleert. |
| TC-CERT-004 | Zertifikat über Domain und SAN finden | Zertifikate/Suche | Muss | Automatisiert/Manuell | Zertifikatseintrag mit mehreren SANs. | Nach CommonName, SAN, Issuer und DeploymentTarget suchen. | Eintrag wird über relevante nicht-geheime Felder gefunden; PrivateKey wird nicht als Klartext in UI/Log ausgegeben. |
| TC-CERT-005 | Ablaufdatum sichtbar, aber keine V1-Automation erzwingen | Zertifikate/UI | Muss | Manuell | Zertifikatseintrag mit ValidTo. | Detailansicht und Liste prüfen. | ValidTo ist sichtbar; V1 muss keine automatische Erneuerung, ACME oder PKI-Workflow anbieten. |
| TC-CERT-006 | Zertifikatsdaten über Speichern/Laden erhalten | Zertifikate/Storage | Muss | Integration | Vollständiger Zertifikatseintrag existiert. | Tresor speichern, schließen, neu öffnen. | Alle Zertifikatsfelder inkl. geheimer Felder bleiben erhalten und korrekt klassifiziert. |
| TC-ORG-001 | Hauptgruppe am Root-Knoten anlegen | Organisation/UI | Muss | Manuell | Tresor geöffnet, Root ausgewählt. | Neue Hauptgruppe anlegen. | Gruppe erscheint auf oberster Ebene; kein Zwang, unter bestehender Gruppe anzulegen. |
| TC-ORG-002 | Untergruppe anlegen und umbenennen | Organisation/UI | Muss | Manuell/Unit | Gruppe existiert. | Untergruppe anlegen, umbenennen, speichern. | Pfad wird korrekt aktualisiert; Einträge im Unterbaum bleiben zugeordnet. |
| TC-ORG-003 | Nicht-leere Gruppe nicht versehentlich löschen | Organisation/Safety | Muss | Manuell/Unit | Gruppe enthält Einträge oder Untergruppen. | Löschvorgang auslösen. | Anwendung verhindert oder bestätigt riskante Aktion klar; kein stiller Verlust. |
| TC-ORG-004 | Eintrag per Drag-and-Drop verschieben | Organisation/DnD | Muss | Manuell/Unit | Eintrag und Zielgruppe existieren. | Eintrag in andere Gruppe ziehen. | Gruppenzuordnung ändert sich; Dirty-State wird gesetzt; Suche/Detail zeigen neue Gruppe. |
| TC-ORG-005 | Gruppe per Drag-and-Drop verschieben | Organisation/DnD | Muss | Manuell/Unit | Hierarchie mit Untergruppen existiert. | Gruppe auf gültige Zielgruppe ziehen. | Pfade werden rekursiv aktualisiert; bei betroffenen Kindern/Einträgen erscheint erforderliche Bestätigung. |
| TC-ORG-006 | Zyklische Gruppenverschiebung verhindern | Organisation/Safety | Muss | Automatisiert/Manuell | Gruppe mit Untergruppe existiert. | Obergruppe auf eigene Untergruppe ziehen. | Aktion wird blockiert; keine beschädigte Hierarchie. |
| TC-TAG-001 | Tags normalisieren und deduplizieren | Tags | Muss | Automatisiert/Manuell | Eintrag wird bearbeitet. | Tags mit unterschiedlicher Groß-/Kleinschreibung und Leerzeichen eingeben. | Tags werden normalisiert; Duplikate verschwinden; bekannte Tags werden gepflegt. |
| TC-TAG-002 | Tag-Klick filtert oder sucht erwartungsgemäß | Tags/UI | Soll für V1 | Manuell | Eintrag mit Tags in Detailansicht. | Tag anklicken. | UI setzt nachvollziehbaren Filter/Suchtext oder führt definierte Tag-Aktion aus. |
| TC-SRC-001 | Case-insensitive Standardsuche | Suche | Muss | Automatisiert/Manuell | Einträge mit gemischter Groß-/Kleinschreibung existieren. | Suchbegriffe in anderer Schreibweise eingeben. | Passende Einträge werden gefunden; keine separate Checkbox für V1 nötig. |
| TC-SRC-002 | Suche über Titel, Benutzer, Tags, Notizen und nicht-geheime Zusatzfelder | Suche | Muss | Automatisiert/Manuell | Verschiedene Eintragstypen existieren. | Nach Werten aus jedem Feldbereich suchen. | Einträge werden gefunden, soweit Feld nicht bewusst von Suche ausgenommen ist. |
| TC-SRC-003 | Typfilter findet Zertifikate getrennt | Suche/Filter | Muss | Manuell/Unit | Mehrere Typen inklusive Zertifikat existieren. | Typfilter HTTPS/TLS-Zertifikat wählen. | Nur passende Zertifikatseinträge werden angezeigt; Filter zurücksetzen zeigt wieder alle relevanten Einträge. |
| TC-SRC-004 | Private Secrets nicht ungeschützt in Suchergebnis darstellen | Suche/Security | Muss | Manuell/Review | Einträge mit geheimen Feldern existieren. | Nach Umgebung oder Titel suchen und Liste prüfen. | Listenansicht zeigt keine Passwörter, Tokens, PFX-Passwörter oder Private Keys im Klartext. |
| TC-CLP-001 | Sensible Werte kopieren und automatisch löschen | Clipboard/Security | Muss | Automatisiert/Manuell | Eintrag mit Secret geöffnet. | Passwort kopieren, Zwischenablage direkt prüfen, nach Timeout erneut prüfen. | Direkt nach Copy ist Wert vorhanden; nach Timeout ist Zwischenablage leer oder sicher überschrieben. |
| TC-CLP-002 | Nicht-sensitive Werte ohne Secret-Warnung kopieren | Clipboard/UI | Muss | Manuell | Eintrag mit Host/URL. | Host oder URL kopieren. | Kopieren funktioniert; keine irreführende Secret-Warnung; Verhalten ist dokumentiert. |
| TC-CLP-003 | Neues Secret überschreibt alten Clipboard-Schutz sauber | Clipboard/Security | Muss | Automatisiert/Manuell | Zwei verschiedene Secret-Werte. | Erstes Secret kopieren, vor Timeout zweites Secret kopieren. | Timer/Policy löscht nicht versehentlich falsche neue Zwischenablage oder lässt alten Wert bestehen; Verhalten ist definiert und getestet. |
| TC-CLP-004 | Clipboard beim Lock bereinigen | Clipboard/Lock | Muss | Manuell/Automatisiert | Secret wurde kopiert. | Tresor sperren. | Sensible Zwischenablage wird bereinigt oder Schutzlogik greift sofort. |
| TC-LOK-001 | Manuell sperren | Lock/Security | Muss | Manuell/Unit | Tresor geöffnet, Detailansicht zeigt Eintrag. | Sperren auslösen. | Details sind leer oder maskiert; Eintragsliste ist geschützt; sensible Aktionen sind deaktiviert. |
| TC-LOK-002 | Mit korrektem Master-Passwort entsperren | Lock/Security | Muss | Manuell/Integration | Tresor gesperrt. | Entsperren mit korrektem Passwort. | Arbeitskontext wird wiederhergestellt; keine Daten gehen verloren. |
| TC-LOK-003 | Falsches Passwort beim Entsperren ablehnen | Lock/Security | Muss | Manuell/Integration | Tresor gesperrt. | Falsches Passwort eingeben. | Tresor bleibt gesperrt; keine Secrets sichtbar; Fehlermeldung bleibt zurückhaltend. |
| TC-LOK-004 | Auto-Lock nach Inaktivität | Lock/Security | Muss | Manuell/Unit | Auto-Lock-Timeout konfiguriert oder Default aktiv. | Anwendung ohne Eingabe warten lassen. | Nach Timeout ist Tresor gesperrt; sichtbare Secrets sind verborgen. |
| TC-LOK-005 | Aktivität setzt Inaktivitätszähler zurück | Lock/UI | Soll für V1 | Manuell/Unit | Auto-Lock aktiv. | Vor Ablauf des Timeouts UI bedienen; warten. | Sperre tritt erst nach tatsächlicher Inaktivität ein, nicht während aktiver Nutzung. |
| TC-GEN-001 | Passwortgenerator erzeugt Standardpasswort | Passwortgenerator | Muss | Automatisiert/Manuell | Generator verfügbar. | Standardoptionen verwenden und Passwort erzeugen. | Passwort erfüllt Länge und Zeichensatzvorgaben; Ergebnis ist nicht leer und wirkt zufällig. |
| TC-GEN-002 | Zeichengruppenoptionen einhalten | Passwortgenerator | Muss | Automatisiert | Generatorservice isoliert testbar. | Varianten mit/ohne Sonderzeichen, Zahlen, Groß-/Kleinbuchstaben testen. | Ergebnisse entsprechen den aktivierten Optionen; ungültige Konfigurationen werden abgelehnt. |
| TC-GEN-003 | Generator übernimmt Ergebnis nur bewusst | Passwortgenerator/UI | Muss | Manuell | Eintrag im Editierdialog geöffnet. | Generator öffnen, Passwort erzeugen, abbrechen; danach wiederholen und übernehmen. | Abbrechen verändert den Eintrag nicht; Übernehmen füllt nur das vorgesehene Feld. |
| TC-GEN-004 | Keine gruppenweiten Default-Passwörter | Passwortgenerator/Security | Muss | Review | Featuremap/Pflichtenheft bekannt. | UI und Code prüfen. | Keine Funktion fördert Passwortwiederverwendung durch gruppenweite Default-Passwörter. |
| TC-DRT-001 | Dirty-State bei Eintragsänderung | Datenverlustschutz | Muss | Automatisiert/Manuell | Tresor geöffnet und sauber. | Eintrag ändern. | Fenstertitel/Status zeigt ungespeichert; Speichern löscht Dirty nach Erfolg. |
| TC-DRT-002 | Dirty-State bei Gruppen- und Drag-and-Drop-Änderung | Datenverlustschutz | Muss | Automatisiert/Manuell | Tresor geöffnet und sauber. | Gruppe umbenennen oder Eintrag verschieben. | Dirty wird gesetzt und bleibt bis erfolgreichem Speichern erhalten. |
| TC-DRT-003 | Dirty-State bei Zertifikatsänderung | Datenverlustschutz/Zertifikate | Muss | Automatisiert/Manuell | Zertifikatseintrag existiert. | ValidTo oder PrivateKeyPassphrase ändern. | Dirty wird gesetzt; Abbrechen verwirft Änderung; Speichern persistiert Änderung. |
| TC-DRT-004 | Warnung bei Beenden mit ungespeicherten Änderungen | Datenverlustschutz/UI | Muss | Manuell | Tresor hat ungespeicherte Änderung. | Fenster schließen. | Rückfrage erscheint; Abbrechen bleibt in Anwendung; Speichern speichert; Verwerfen ist bewusst. |
| TC-DRT-005 | Warnung bei Öffnen eines anderen Tresors | Datenverlustschutz/UI | Muss | Manuell | Aktueller Tresor hat ungespeicherte Änderung. | Anderen Tresor öffnen. | Rückfrage erscheint; keine stillen Verluste. |
| TC-IMP-001 | .psafe3-Testdatei importieren | Password-Safe-Import | Muss | Integration/Manuell | Künstliche .psafe3-Testdatei ohne echte Secrets vorhanden. | Import-Assistent starten, Datei importieren. | Import erzeugt interne Einträge und Importbericht; Anwendung bleibt im internen Modell. |
| TC-IMP-002 | Gruppen, Titel, Benutzername, Passwort, URL und Notizen mappen | Password-Safe-Import | Muss | Integration | Testdatei mit bekannten Feldern. | Import durchführen und Ergebnis prüfen. | Direkt abbildbare Felder landen an erwarteter Stelle. |
| TC-IMP-003 | Nicht abbildbare Felder nicht verschlucken | Password-Safe-Import | Muss | Integration/Review | Testdatei mit Sonder-/Zusatzdaten. | Import durchführen. | Importbericht nennt nicht direkt abbildbare Felder; Daten werden sinnvoll als Zusatzfeld oder markierter Hinweis übernommen. |
| TC-IMP-004 | Falsches Passwort oder defekte .psafe3-Datei behandeln | Password-Safe-Import | Muss | Integration/Manuell | Defekte oder falsch entschlüsselbare Testdatei. | Import versuchen. | Fehler wird klar gemeldet; kein partieller Erfolg ohne Hinweis; keine Secrets im Fehlertext. |
| TC-IMP-005 | Zertifikatseinträge werden nicht fälschlich aus .psafe3 erfunden | Password-Safe-Import/Zertifikate | Soll für V1 | Review/Manuell | Import aus Password Safe. | Importiertes Ergebnis prüfen. | Nur eindeutige Daten werden gemappt; Zertifikatstyp wird nicht automatisch geraten, wenn Semantik unsicher ist. |
| TC-LOG-001 | Keine Secrets in Debug- und Session-Logs | Logging/Security | Muss | Manuell/Automatisiert | Testlauf mit Login, API, Zertifikat und Import. | Logs nach Dummy-Passwörtern, Token, PrivateKey, PFX-Passwort durchsuchen. | Keine sensiblen Werte erscheinen im Klartext; Redaction-Regeln greifen. |
| TC-LOG-002 | Fehlerdialoge geben keine Secrets preis | Logging/UI | Muss | Manuell | Fehler mit Eintrag auslösen, z. B. Storage-Fehler oder Importfehler. | Dialoginhalt prüfen. | Dialog ist verständlich, aber enthält keine Secrets oder vollständigen Connection Strings mit Passwörtern. |
| TC-DOC-001 | README passt zum tatsächlichen V1-Stand | Dokumentation | Muss | Review | V1-Release-Kandidat steht. | README gegen Featuremap/Roadmap prüfen. | README verspricht keine V2/V3-Funktionen als fertig; Sicherheitsstatus ist klar. |
| TC-DOC-002 | Security-Hinweise und Known Limitations vorhanden | Dokumentation/Security | Muss | Review | Release-Dokumentation liegt vor. | Prüfen, ob Schutzgrenzen, kein Malware-Schutzversprechen, kein Recovery-Backdoor, keine Produktivfreigabe vor V1 klar beschrieben sind. | Grenzen sind klar, ehrlich und konsistent dokumentiert. |
| TC-DOC-003 | Benutzerkurzdoku beschreibt sichere Grundabläufe | Dokumentation/Usability | Soll für V1 | Review/Manuell | Benutzerkurzdoku vorhanden. | Abläufe Tresor anlegen, öffnen, speichern, Zertifikatseintrag, Copy, Lock, Backup prüfen. | Dokumentation ist für reale Nutzung ausreichend verständlich. |


# Manuelle Abnahmeszenarien V1

Die Abnahmeszenarien bündeln mehrere Detailtests zu realistischen Nutzerläufen. Sie sind vor einem V1-Release vollständig durchzuführen, auch wenn automatisierte Tests bereits grün sind.


| ID | Szenario | Ablauf | Abnahmekriterium |
| --- | --- | --- | --- |
| AK-V1-01 | Erster sicherer Arbeitslauf | Neuen Tresor anlegen, Login, Datenbank und Zertifikatseintrag erfassen, speichern, schließen, erneut öffnen. | Alle Daten bleiben vollständig; Secrets sind maskiert; keine echten Secrets notwendig. |
| AK-V1-02 | Datenverlustschutz im Alltag | Einträge und Gruppen ändern, dann Neu/Öffnen/Beenden auslösen und alle Antwortpfade prüfen. | Keine Änderung geht ohne bewusste Entscheidung verloren. |
| AK-V1-03 | Zertifikatsverwaltung V1 | HTTPS/TLS-Zertifikat mit Domain, SANs, ValidTo, Issuer, Fingerprint, PrivateKeyPassphrase und PfxPassword erfassen. | Nicht-geheime Zertifikatsdaten sind sichtbar und suchbar; geheime Felder bleiben geschützt. |
| AK-V1-04 | Clipboard und Lock | Passwort, API-Token und PFX-Passwort kopieren; Auto-Clear und manuelles Sperren prüfen. | Zwischenablage wird geschützt; nach Lock sind keine Secrets sichtbar. |
| AK-V1-05 | Backup und Fehlerfälle | Tresor überschreiben, Backup prüfen, defekte Kopie öffnen, falsches Passwort testen. | Backups bleiben verschlüsselt; Fehler sind verständlich; keine Abstürze. |
| AK-V1-06 | Organisation und Wiederfinden | Gruppen, Untergruppen, Tags, Drag-and-Drop, Typfilter und Suche mit technischen Einträgen prüfen. | Einträge bleiben korrekt zugeordnet und sind zuverlässig auffindbar. |
| AK-V1-07 | Password-Safe-Import | Künstliche .psafe3-Datei importieren und Importbericht prüfen. | Abbildbare Daten werden übernommen; nicht abbildbare Daten werden sichtbar gemeldet. |
| AK-V1-08 | Release- und Dokumentationsprüfung | Build/Test, README, Security-Hinweise, Featuremap, Roadmap und Known Limitations prüfen. | Dokumentation stimmt mit tatsächlichem Funktionsstand überein. |


# Testdaten und sichere Testumgebung

Für Tests dürfen keine echten Passwörter, produktiven API-Tokens, echten Kunden-Zertifikate, realen privaten Schlüssel oder produktiven PFX-Dateien verwendet werden. Zertifikatstests nutzen selbst erzeugte Dummy-Zertifikate oder reine Textplatzhalter. Dummy-Secrets sollten eindeutig als Testwerte erkennbar sein, damit Log- und Dateiprüfungen zuverlässig danach suchen können.

Empfohlene Teststruktur:

- test-vault-basic.svault für klassische Logins und technische Einträge.
- test-vault-certificates.svault für HTTPS/TLS-Zertifikatseinträge.
- test-vault-corrupt.svault als beschädigte Datei.
- test-import-passwordsafe.psafe3 als künstliche Importdatei.
- test-output/logs/ für temporäre Entwicklungslogs, die nach jedem Testlauf auf Secret-Leaks geprüft werden.

Alle Testdateien müssen künstlich sein. Falls Testdateien im Repository verbleiben, dürfen sie weder echte Secrets noch reale personenbezogene Zugangsdaten enthalten.


# Automatisierungsstrategie

Nicht jeder Test muss sofort vollständig automatisiert werden. Für V1 sollten jedoch alle sicherheitsnahen Kernregeln automatisiert oder zumindest isoliert testbar sein. Dazu gehören Storage, Krypto-Helfer, Passwortgenerator, Dirty-State-Regeln, Organisationslogik, Suche, Clipboard-Policy, Lock-State-Policy, Zertifikatsfeldklassifizierung und Import-Mapping.

Manuelle Tests bleiben für die Oberfläche notwendig. Dennoch sollte die UI möglichst dünn bleiben: MainForm und Dialoge orchestrieren, die eigentlichen Regeln liegen in Application- und Security-Services. Dadurch können spätere Regressionen schneller erkannt werden.


# V1-Freigabeentscheidung

Eine V1-Freigabe ist nur vertretbar, wenn alle Muss-Release-Gates bestanden sind, keine S0- oder S1-Fehler offen sind, alle kritischen Security-Tests bestanden wurden und die Dokumentation keine falschen Versprechen enthält.

Offene S2-Fehler dürfen nur dann in V1 verbleiben, wenn sie nicht sicherheitskritisch sind, einen klaren Workaround besitzen und in den Release Notes dokumentiert werden. S3- und S4-Fehler können in die V1.x-Politur verschoben werden, wenn sie keine Sicherheitsgrenze und keinen Datenverlust betreffen.


# Bewusste Nicht-Ziele der V1-Abnahme

Die folgenden Punkte sind nicht Teil der V1-Abnahme und dürfen nicht als fehlgeschlagene V1-Tests gewertet werden: Cloud-Sync, Team-Sharing, Mobile-Client, Browser-Autofill, automatische ACME-Erneuerung, eigene PKI, Zertifikats-Deployment auf Server, Certificate-Transparency-Monitoring, Recovery-Konzept ohne Backdoor, Passkeys/WebAuthn, vollständiger Password-Safe-Roundtrip, CSV-Export, Browser-Import und globale Multi-Vault-Suche.

Diese Abgrenzung ist wichtig, damit V1 klein, sicher und glaubwürdig bleibt. Spätere Funktionen erhalten eigene Testkataloge, sobald sie in V1.x, V2 oder V3 konkret geplant werden.
