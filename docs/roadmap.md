# Roadmap: SASD Desktop Secret Manager

**Überarbeitete Roadmap passend zu Lastenheft, Pflichtenheft und Architekturupdate**  
Stand: 18.05.2026

> Diese Roadmap konsolidiert die bisherige Milestone-Linie mit den nachträglich aufgenommenen Anforderungen, insbesondere der Verwaltung von HTTPS/TLS-Zertifikaten. Sie ersetzt nicht Lastenheft, Pflichtenheft oder Architektur, sondern übersetzt sie in eine umsetzbare Reihenfolge.

## 1. Management Summary

Der bisherige Prototyp ist als V0.x-Stand einzuordnen: wichtige Grundlagen wie .svault, WinForms-Oberfläche, Gruppen, Tags, Suche, CRUD, Drag-and-Drop und Tests sind vorhanden. Für eine runde V1 fehlen jedoch noch Alltagssicherheit, Datenverlustschutz, Locking, Passwortgenerator, Zertifikatseinträge, verbesserte Suche, Backup-Härtung, Password-Safe-Import und Release-Härtung.

Die wichtigste Roadmap-Änderung ist die Aufnahme der HTTPS/TLS-Zertifikatsverwaltung als eigener V1-Baustein. Zertifikate werden in V1 strukturiert erfasst und sicher verwaltet; automatische ACME-/PKI-/Deployment-Funktionen bleiben V3/später.

## 2. Leitentscheidungen

- V1 bleibt lokal, offline-first, Single-User und Windows-Desktop-orientiert.
- Das interne .svault-Modell bleibt führend; Password Safe ist Interop, nicht Primärformat.
- HTTPS/TLS-Zertifikate sind V1-relevant, aber ACME/PKI-Automation ist kein V1-Ziel.
- Sicherheits- und Datenverlustschutz kommen vor Komfort- und Präsentationsfunktionen.
- Neue Fachlogik wird in Application-/Security-Services gekapselt und nicht in MainForm versteckt.
- V1.x und V2 bleiben sichtbar geplant, dürfen aber V1 nicht blockieren.

## 3. Zielbild der Versionen

| Version | Ziel | Enthält | Nicht enthalten |
|---|---|---|---|
| V0.x | Aktueller Prototyp | .svault, UI-Grundfunktionen, Gruppen, Tags, Suche, CRUD, Drag-and-Drop, Tests | Keine produktive Freigabe |
| V1.0 | Runde lokale Erstversion | Clipboard, Dirty-Schutz, Auto-Lock, Passwortgenerator, Zertifikatseinträge, Backup-Härtung, Suche/Filter, .psafe3-Import, Release-Härtung | Cloud, Team, Mobile, Browser-Autofill, ACME/PKI |
| V1.x | Härtung und Produktpolitur | Master-Passwort ändern, Generatorprofile, Tag-Verwaltung, Zertifikatswarnungen, Restore-Komfort, Produktpolitur | Kein Architekturbruch |
| V2 | Funktionsausbau | Cross-Vault, Historie, TOTP, Anhänge, Mehrsprachigkeit, Themes, optionale Online-Funktionen | Keine Pflicht zur Cloud |
| V3/später | Heikle Erweiterungen | Browser-Import, Passkeys, ACME/PKI, Team/Cloud/Mobile, Recovery-Konzept | Nur nach separater Bewertung |

## 4. Roadmap bis V1.0

### M12 – Clipboard-Schutz und Secret-Komfort
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Die Zwischenablage ist im Alltag einer der riskantesten Offenlegungspunkte. Vor Zertifikats-Private-Key-Feldern muss eine zentrale Schutzlogik stehen.

**Umfang:**
- Zentraler ClipboardProtectionService statt verteilter Timer-Logik in Controls.
- Trennung zwischen sensiblen und nicht-sensiblen Copy-Aktionen.
- Auto-Clear nach konfigurierbarer Standardzeit; Statusmeldung nach Copy und nach Clear.
- Keine Secrets in Logs, Fehlermeldungen oder Debug-Ausgaben.

**Nicht enthalten:**
- Kein Schutzversprechen gegen Malware oder Keylogger.
- Keine globale Betriebssystem-Härtung.

**Abnahmekriterien:**
- Sensible Felder werden kopiert und nach Ablauf der definierten Zeit geleert.
- Nicht-sensitive Felder wie Host oder URL werden ohne Secret-Warnung behandelt.
- Tests prüfen Timer-/Policy-Logik ohne fragile UI-Abhängigkeit.

**Commit-Linie:**
- Add central clipboard protection service
- Wire sensitive copy actions through clipboard protection
- Add tests for clipboard auto-clear policies

### M13 – Ungespeicherte Änderungen und Schutz vor Datenverlust
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Die Anwendung verwaltet wertvolle Secrets. Datenverlust durch Neu, Öffnen, Beenden oder fehlerhafte Dirty-State-Logik wäre ein schwerer Vertrauensbruch.

**Umfang:**
- Dirty-State konsistent über Eintrags-, Gruppen-, Tag-, Drag-and-Drop- und Zertifikatsänderungen.
- Rückfragen bei Neu, Öffnen, Schließen und Beenden.
- Statusleiste, Fenstertitel und Menüstatus sauber synchronisieren.
- Abbruchpfade dürfen keine Daten still verwerfen.

**Nicht enthalten:**
- Noch kein komplexes Undo-System.
- Noch kein automatisches Merge bei parallelen Änderungen.

**Abnahmekriterien:**
- Jeder fachlich relevante Änderungspfad setzt Dirty.
- Speichern löscht Dirty nur nach erfolgreichem Schreiben.
- Abbrechen in Dialogen verändert den Tresor nicht.

**Commit-Linie:**
- Add unsaved changes guard for new open and exit workflows
- Refine dirty state synchronization across vault actions
- Add tests for mutation dirty behavior

### M14 – Tresor sperren, entsperren und Auto-Lock
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Ein Secret Manager muss Arbeitsunterbrechungen sicher behandeln. Offene Detailansichten, kopierte Werte und entsperrte Tresore dürfen nicht unbegrenzt sichtbar bleiben.

**Umfang:**
- Manuelle Sperrfunktion für den aktiven Tresor.
- Auto-Lock nach Inaktivität, optional später bei Minimieren.
- Entsperren nur mit Master-Passwort des aktiven Tresors.
- Beim Sperren Detailansicht leeren bzw. maskieren, Copy-Zustände zurücksetzen.

**Nicht enthalten:**
- Kein Windows-Biometrie-/Hello-Login in V1.
- Kein Multi-User-Lockmodell.

**Abnahmekriterien:**
- Nach Lock sind keine Secrets sichtbar.
- Falsches Passwort entsperrt nicht.
- Nach korrektem Passwort wird der Arbeitskontext wiederhergestellt.

**Commit-Linie:**
- Add vault lock and unlock workflow
- Add inactivity based auto-lock for open vaults
- Clear sensitive UI state when vault locks

### M15 – Passwortgenerator Basismodul
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Der Generator verhindert, dass Nutzer neue schwache oder wiederverwendete Passwörter manuell erfinden müssen. Gruppenweite Default-Passwörter bleiben ausdrücklich verworfen.

**Umfang:**
- Generatorservice außerhalb der UI.
- Dialog mit Länge, Zeichengruppen und optionalen Ausschlüssen.
- Einfügen in Secret-Feld und geheime Zusatzfelder.
- Sichere Standardwerte statt zu vieler verwirrender Optionen.

**Nicht enthalten:**
- Noch keine komplexen Organisationsprofile je Kunde.
- Noch keine firmweiten Richtlinien.

**Abnahmekriterien:**
- Generator erzeugt reproduzierbar regelkonforme, zufällige Passwörter.
- UI übernimmt Ergebnis nur bewusst.
- Tests prüfen Längen, Zeichengruppen und Randfälle.

**Commit-Linie:**
- Add password generator service
- Add password generator dialog with safe defaults
- Add tests for password generation options

### M16 – Datenmodell-Konsolidierung und Secret-Typen
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Vor spezialisierten Dialogen muss das Modell sauber festgelegt sein. HTTPS/TLS-Zertifikate dürfen nicht als zufällige Notiz- oder Custom-Hacks entstehen.

**Umfang:**
- EntryType HttpsTlsCertificate ergänzen.
- Feldkonventionen für Zertifikate, Datum, mehrzeilige Texte und geheime Schlüsselfelder festlegen.
- ModelVersion und Migrationsverhalten prüfen.
- Bestehende EntryTypes und CustomFieldKinds bereinigen.

**Nicht enthalten:**
- Keine neue Klassenhierarchie pro Secret-Typ.
- Keine externe Datenbank.

**Abnahmekriterien:**
- Alte Tresore laden weiterhin.
- Neue EntryTypes werden gespeichert und wieder geladen.
- Unbekannte oder spätere Feldarten führen nicht zu Datenverlust.

**Commit-Linie:**
- Add HTTPS TLS certificate entry type
- Consolidate custom field kinds and field conventions
- Add model version tests for new entry types

### M17 – HTTPS/TLS-Zertifikatseinträge V1
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Die Zertifikatsverwaltung ist die wichtigste neue Anforderung aus den späteren Chats und muss sichtbar in V1 landen, ohne zu einer PKI-Suite auszuufern.

**Umfang:**
- Editierbereich für Zertifikatseinträge mit Identität, SANs, Gültigkeit, Aussteller/Fingerprint, Deployment und Secret-Key-Bereich.
- Detailansicht mit Domain, ValidTo, Issuer, SANs und maskierten sensiblen Feldern.
- Sichere Behandlung von PFX/P12-Passwort, Private-Key-Passphrase und PrivateKeyPem.
- Suche über Domain, SANs, Issuer und DeploymentTarget vorbereiten.

**Nicht enthalten:**
- Keine automatische ACME-Erneuerung.
- Keine PKI-Verwaltung.
- Keine automatischen Server-Deployments.
- Keine Certificate-Transparency-Überwachung.

**Abnahmekriterien:**
- Ein Zertifikatseintrag kann vollständig manuell erfasst, gespeichert, geschlossen und wieder geöffnet werden.
- Private-Key-/PFX-Felder bleiben maskiert und laufen über Clipboard-Schutz.
- Ablaufdatum und Domain sind ohne Secret-Reveal sichtbar.

**Commit-Linie:**
- Add certificate entry template and view model
- Add certificate fields panel to entry edit dialog
- Add tests for certificate secret redaction and search fields

### M18 – Backup-, Restore- und Dateikonflikt-Härtung
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Ein Secret Manager darf bei Speicherfehlern nicht vertrauenswürdig wirken, wenn die Dateikette unsauber ist. Backups müssen verschlüsselt bleiben und Konflikte klar melden.

**Umfang:**
- Atomisches Speichern nochmals überprüfen und dokumentieren.
- Backup-Datei beim Überschreiben; klare UI-Hinweise zu Backup-Orten.
- Fehlerfälle für falsches Passwort, beschädigte Datei, abgeschnittene Datei und Dateizugriff.
- Konservative Konflikterkennung bei Zweitöffnung derselben Datei.

**Nicht enthalten:**
- Noch kein komfortabler Restore-Assistent mit Historie.
- Noch kein Cloud-/Sync-Konfliktmerge.

**Abnahmekriterien:**
- Defekte Dateien werden sauber gemeldet.
- Backup-Dateien enthalten keine Klartextdaten.
- Parallele Schreibkonflikte werden verhindert oder deutlich angezeigt.

**Commit-Linie:**
- Improve backup and conflict handling for vault files
- Add corruption and file access error handling
- Document encrypted backup behavior

### M19 – Suche, Filter und Tag-Verhalten alltagstauglich machen
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Mit technischen Secrets und Zertifikaten reicht einfache Titelsuche nicht mehr. Wiederauffindbarkeit ist Kernnutzen.

**Umfang:**
- Suche über Titel, Benutzer/Principal, Gruppen, Tags, Typen, Notizen und nicht-geheime Zusatzfelder.
- Typfilter und Tagfilter, besonders für Zertifikate, API, Datenbank und Mail.
- Root-Ansicht mit verständlicher Gesamtübersicht.
- Zertifikatssuche über CN/Domain, SANs, Issuer und DeploymentTarget.

**Nicht enthalten:**
- Noch kein Volltextindex mit separater Indexdatei.
- Noch kein globaler Cross-Vault-Search.

**Abnahmekriterien:**
- Einträge werden über Zusatzfelder gefunden.
- Typ-/Tagfilter können kombiniert werden.
- Geheime Felder werden nicht versehentlich in Listen oder Logs ausgegeben.

**Commit-Linie:**
- Expand search to notes tags and custom fields
- Add entry type and tag filters
- Add certificate field search coverage

### M20 – Password-Safe-Import (.psafe3) mit Bericht
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Die Herkunft aus Password Safe bleibt wichtig. Der Import muss ehrlich sein und darf das interne Modell nicht auf Fremdformatgrenzen reduzieren.

**Umfang:**
- Reader/Importer über Interop.PasswordSafe-Schicht.
- Mapping auf internes Modell mit Gruppen, Titel, Benutzer, Secret, URL, Notizen und Zusatzfeldern.
- Importbericht mit Warnungen zu nicht abbildbaren Feldern.
- Import in neuen oder bestehenden Tresor kontrolliert ermöglichen.

**Nicht enthalten:**
- Kein vollständiger Roundtrip.
- Kein .psafe3 als primäres Arbeitsformat.
- Kein verlustbehafteter Export ohne Bericht.

**Abnahmekriterien:**
- Testdatei wird importiert.
- Mappingbericht ist nachvollziehbar.
- Nicht abbildbare Inhalte verschwinden nicht still.

**Commit-Linie:**
- Add initial Password Safe reader and import mapping
- Add import wizard and compatibility reporting
- Add psafe import tests for lossy fields

### M21 – V1-Release-Härtung, Dokumentation und Paketierung
**Phase:** V1.0 Abschluss · **Priorität:** Muss

**Warum:** Vor einer ernsthaften Nutzung braucht das Projekt einen nachvollziehbaren Freigabestand mit Tests, Security-Review, Benutzerhinweisen und sauberer Dokumentation.

**Umfang:**
- End-to-End-Testliste und manueller QA-Durchlauf.
- Security-Review für Logs, Clipboard, Auto-Lock, Export/Import und Zertifikatsfelder.
- README, Lastenheft, Pflichtenheft, Architektur, Roadmap und Benutzerkurzdoku aktualisieren.
- Release-Notizen, Screenshots, Icon/Branding falls schon stabil.

**Nicht enthalten:**
- Kein Marketing-Overkill.
- Keine öffentlichen Sicherheitsversprechen über das reale Schutzmodell hinaus.

**Abnahmekriterien:**
- dotnet restore/build/test grün.
- Manueller V1-Testkatalog bestanden.
- Dokumente und Repository sprechen dieselbe Sprache.

**Commit-Linie:**
- Harden release candidate for desktop secret manager v1
- Finalize documentation and packaging for v1 release
- Add manual QA checklist for v1

## 5. V1.x-Roadmap

| Milestone | Thema | Version | Beschreibung |
|---|---|---|---|
| M22 | Master-Passwort ändern und Re-Encryption | V1.x | Altes Passwort validieren, neues Passwort bewerten, Tresor mit neuem Schlüssel speichern, Fehlerfälle testen. Kein stiller Passwortwechsel ohne erfolgreiches Backup. |
| M23 | Generatorprofile und erweiterte Eintragsvorlagen | V1.x | Profile für typische Passwortlängen und Zeichensätze; Templates für Login, Datenbank, Mail, FTP/SFTP, API, Server und Zertifikat. Keine gruppenweiten Default-Passwörter. |
| M24 | Tag-Verwaltung und Bereinigungswerkzeuge | V1.x | Tag-Liste verwalten, Tags umbenennen/zusammenführen, unbenutzte Tags entfernen, Duplikate und inkonsistente Schreibweisen reduzieren. |
| M25 | Zertifikatsablauf-Warnungen und Zertifikatsübersicht | V1.x | Warnschwellen z. B. 30/14/7 Tage, Liste bald ablaufender Zertifikate, klare Anzeige ohne automatische Erneuerungsversprechen. |
| M26 | Restore-Komfort, rotierende Backups und LocalAppData-Sicherung | V1.x | Backup-Kette sichtbar machen, verschlüsselte zusätzliche Sicherungen, Restore-Hinweise. Keine Klartext-Exporte. |
| M27 | Produktpolitur und Integritätsprüfungen | V1.x | Eigenes Icon, konsistente Dialoge, kleinere Tastaturkürzel, begrenzte Hash-/Manifestprüfung eigener Dateien mit ehrlicher Schutzgrenze. |
| M28 | Optionale .psafe3-Export-Vorbereitung | V1.x/V2 | Nur für klar abbildbare Daten und immer mit Kompatibilitätsbericht. Zertifikatseinträge voraussichtlich nur verlustbehaftet; deshalb nicht erzwingen. |

## 6. V2-Roadmap

| Milestone | Thema | Beschreibung |
|---|---|---|
| M30 | Cross-Vault-Komfort | Einträge zwischen geöffneten Tresoren kopieren, read-only Zweittresore, spätere Multi-Vault-Suche. Keine parallele unkontrollierte Schreibbearbeitung. |
| M31 | Passwort- und Secret-Historie | Kontrollierte Historie mit Begrenzung und Sicherheitswarnung. Alte Secrets erhöhen Risiko und brauchen klare Policies. |
| M32 | TOTP-Verwaltung | Optionaler Komfort, aber mit Hinweis auf geringere Faktortrennung, wenn Passwort und TOTP im selben Tresor liegen. |
| M33 | Verschlüsselte Anhänge | Zertifikatsdateien wie CRT/CER/PFX/Chain-Dateien und andere Anhänge verschlüsselt im Tresor ablegen; keine Klartextdateien im Arbeitsverzeichnis. |
| M34 | Mehrsprachigkeit und Themes | Deutsch/Englisch über Ressourcenmechanismus, Hell/Dunkel-Theme, ohne Fachlogik in UI-Strings zu verstecken. |
| M35 | Opt-in Online-Funktionen | Passwort-Leak-Prüfung, Update-Suche, News-Anzeige nur optional, transparent und datenschutzbewusst. |

## 7. V3/später

| Milestone | Thema | Beschreibung |
|---|---|---|
| M40 | Browser-Import | Import aus Browsern erst nach Datenschutz-, Plattform- und Sicherheitsbewertung. Kein heimliches Auslesen. |
| M41 | Passkeys/WebAuthn-Konzept | Separates Konzept nötig, da Passkeys nicht wie klassische Passwörter modelliert werden sollten. |
| M42 | ACME-/PKI-/Deployment-Automation | Mögliche spätere Schwesterfunktion, nicht V1. Erfordert eigene Sicherheits-, Betriebs- und Haftungsbetrachtung. |
| M43 | Team-/Cloud-/Mobile-Strategie | Erst nach Produktreife des lokalen Kerns; berührt Rollen, Offboarding, Synchronisation, Datenschutz und Betrieb. |
| M44 | Recovery ohne Backdoor | Offizieller Recovery-Mechanismus nur als bewusst eingerichtetes Verfahren, niemals als versteckte Hintertür. |

## 8. Anforderungs-Mapping

| Anforderung | Roadmap-Entscheidung | Milestone |
|---|---|---|
| Mehrere lokale Tresore | Bereits V0.x angelegt, V1 absichern | M13, M18, M21 |
| Clipboard-Autoclear | V1 Muss | M12 |
| Lock/Auto-Lock | V1 Muss | M14 |
| Passwortgenerator | V1 Basis, V1.x Profile | M15, M23 |
| HTTPS/TLS-Zertifikate | V1 Typ/Erfassung, V1.x Warnungen, V2 Anhänge, V3 ACME | M16, M17, M25, M33, M42 |
| Backups und Restore-Sicherheit | V1 robuste Basis, V1.x Komfort | M18, M26 |
| Suche/Filter/Tags | V1 alltagstauglich, V1.x Verwaltung | M19, M24 |
| Password-Safe-Import | V1 Import mit Bericht | M20 |
| Password-Safe-Export | Nicht V1; nur kontrolliert später | M28 |
| Master-Passwort ändern | V1.x Folgeausbau | M22 |
| Mehrsprachigkeit/Theme | V2 | M34 |
| Passwort-Historie/TOTP/Anhänge | V2 | M31, M32, M33 |
| Browser-Import/Passkeys/PKI/Team | V3/später nach separatem Konzept | M40-M43 |

## 9. Risiken und Gegenmaßnahmen

| Risiko | Bedeutung | Maßnahme |
|---|---|---|
| Scope Creep durch Zertifikate | Zertifikate in V1 nur strukturieren, nicht erneuern oder deployen. | M16/M17 sauber abgrenzen; ACME/PKI in M42 verschieben. |
| Secret-Leaks in Logs oder Clipboard | Private Keys, PFX-Passwörter und Tokens sind besonders kritisch. | M12 vor M17 abschließen; Redaction-Tests einplanen. |
| MainForm wächst weiter monolithisch | Neue UI-Funktionen dürfen nicht die Form-Klasse aufblasen. | Application-Services und ViewModels pro Feature. |
| Import verliert Daten still | Password-Safe-Interop darf nicht vertrauensschädlich sein. | Importbericht und Negativtests in M20. |
| V1 dauert zu lange | Zu viele Komfortfunktionen können Release verhindern. | V1 strikt auf lokale runde Erstversion begrenzen; V1.x/V2 sichtbar parken. |

## 10. Nächste konkrete Arbeitspakete

1. M12 vollständig abschließen und Clipboard-Schutz zentralisieren.
2. M13 Dirty-State und Datenverlustschutz über alle Aktionen stabilisieren.
3. M14 Lock/Auto-Lock implementieren, bevor weitere Secret-Felder hinzukommen.
4. M15 Passwortgenerator einbauen.
5. M16/M17 Zertifikatsmodell und Zertifikats-UI sauber umsetzen.

Diese Reihenfolge reduziert Sicherheitsrisiken, bevor die neue fachliche Breite durch HTTPS/TLS-Zertifikate in die UI kommt.