# Abnahme-Checkliste V1

## Basisfunktionen

- [ ] Neuer Tresor kann angelegt werden.
- [ ] Tresor kann gespeichert, geschlossen und erneut geöffnet werden.
- [ ] Falsches Master-Passwort wird abgelehnt.
- [ ] Mehrere Tresordateien können getrennt genutzt werden.
- [ ] Einträge können angelegt, bearbeitet und gelöscht werden.
- [ ] Gruppen und Untergruppen können verwaltet werden.
- [ ] Tags können verwendet und gesucht werden.
- [ ] Drag & Drop für Einträge funktioniert kontrolliert.
- [ ] Drag & Drop für Gruppen verhindert Zyklen.

## Sicherheitsfunktionen

- [ ] Secrets werden standardmäßig maskiert.
- [ ] Reveal-Aktionen sind bewusst und reversibel.
- [ ] Sensible Copy-Aktionen lösen Clipboard-Autoclear aus.
- [ ] Auto-Lock sperrt den Tresor nach Inaktivität.
- [ ] Logs enthalten keine Secrets.
- [ ] Backup-Dateien sind verschlüsselt.
- [ ] Beschädigte Dateien werden nicht als gültiger Tresor akzeptiert.

## Zertifikatsverwaltung

- [ ] Zertifikatseintrag kann angelegt werden.
- [ ] Common Name, SANs, Issuer, Seriennummer, Valid From/To und Fingerprints können strukturiert erfasst werden.
- [ ] Private Key, PFX und Passphrase werden als sensible Felder behandelt.
- [ ] Zertifikatseinträge sind über Suche und Filter auffindbar.
- [ ] Ablaufdatum ist erfassbar und für spätere Warnlogik nutzbar.

## Interop

- [ ] `.psafe3`-Import ist kontrolliert testbar.
- [ ] Importbericht zeigt nicht abbildbare Informationen an.
- [ ] Es gibt keinen stillen Datenverlust.

## Dokumentation

- [ ] README, Lastenheft, Pflichtenheft, Architektur, Featuremap, Roadmap, Security-/Threat-Model und Testkatalog sind konsistent.
- [ ] Nicht-Ziele sind klar dokumentiert.
- [ ] Bekannte Grenzen sind ehrlich beschrieben.
