# Security-Checkliste V1

## Vor jedem V1-Release prüfen

| Nr. | Prüfung | Muss erfüllt sein? |
|---:|---|---|
| S-01 | Keine echten Secrets, Tokens, Passwörter, Private Keys oder PFX-Dateien im Repository. | Ja |
| S-02 | Debug-Logs enthalten keine Klartext-Secrets und keine Master-Passwörter. | Ja |
| S-03 | `.svault` wird verschlüsselt gespeichert und kann mit falschem Passwort nicht geöffnet werden. | Ja |
| S-04 | Beschädigte oder abgeschnittene `.svault`-Dateien werden sauber abgelehnt. | Ja |
| S-05 | Sensible Felder sind standardmäßig maskiert. | Ja |
| S-06 | Clipboard-Autoclear funktioniert für sensible Copy-Aktionen. | Ja |
| S-07 | Auto-Lock verdeckt oder entfernt sichtbare Secrets. | Ja |
| S-08 | Master-Passwort wird nicht gespeichert und nicht in App-Einstellungen abgelegt. | Ja |
| S-09 | Backups enthalten keine unverschlüsselten Nutzdaten. | Ja |
| S-10 | Password-Safe-Import erzeugt keine stillen Datenverluste. | Ja |
| S-11 | HTTPS-/TLS-Zertifikatseinträge behandeln Private Key, PFX und Passphrase als hochkritisch. | Ja |
| S-12 | CSV-/Klartextexporte sind in V1 nicht aktiv oder nur mit expliziter Warnlogik in späterer Version vorgesehen. | Ja |

## Nicht durch V1 versprochen

- Schutz gegen vollständig kompromittierte Betriebssysteme.
- Schutz gegen Keylogger, RAM-Auslese oder Malware im Benutzerkontext.
- Cloud-Synchronisation, Team-Sharing oder zentrale Rechteverwaltung.
- Recovery über eine versteckte Hersteller-Backdoor.
