# DSM-006 – HTTPS-/TLS-Zertifikatsverwaltung

## Ziel

HTTPS-/TLS-Zertifikate werden als eigener Secret-Typ verwaltet, damit Zertifikatsinformationen nicht unstrukturiert in Notizen verschwinden.

## V1-Umfang

- EntryType `Certificate` oder vergleichbare Typisierung.
- Strukturierte Felder: Common Name, SANs, Issuer, Seriennummer, Valid From, Valid To, Fingerprints, Algorithmus, Key Usage, Umgebung, System/Host.
- Sensible Felder: Private Key, PFX/PKCS#12, Passphrase, ggf. Deployment Secret.
- Zertifikatseinträge sind such- und filterbar.
- Private-Key-/PFX-Felder werden maskiert und durch Clipboard-Schutz behandelt.

## V1.x

- Ablaufwarnungen.
- Zertifikatsübersicht nach Ablaufdatum.
- Hinweise auf bald ablaufende Zertifikate.

## V2

- Verschlüsselte Anhänge für CRT/PEM/PFX/CSR.
- Fingerprint-Berechnung aus importierten Zertifikatsdateien.

## V3/später

- ACME-/Let's-Encrypt-Unterstützung.
- PKI-/CA-Workflows.
- Deployment-Automation.

## Tests

- Zertifikatseintrag kann angelegt und gespeichert werden.
- Sensible Zertifikatsfelder bleiben maskiert.
- Ablaufdatum kann gesucht/gefiltert werden.
- Keine Private Keys in Logs.
