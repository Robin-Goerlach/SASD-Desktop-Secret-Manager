# ADR-006: HTTPS-/TLS-Zertifikate als eigener Secret-Typ

## Status

Akzeptiert

## Kontext

Im Projektverlauf kam die Anforderung hinzu, HTTPS-/TLS-Zertifikate zu verwalten. Zertifikate besitzen andere fachliche Strukturen als klassische Logins und enthalten teils hochkritische Bestandteile wie Private Keys oder PFX-Dateien.

## Entscheidung

HTTPS-/TLS-Zertifikate werden als eigener Secret-Typ modelliert. Öffentliche Zertifikatsmetadaten und hochkritische Schlüsselmaterialien werden sauber getrennt klassifiziert.

## Konsequenzen

- V1 enthält strukturierte Zertifikatsfelder.
- Private Key, PFX/PKCS#12 und Passphrase werden als Secrets behandelt.
- Ablaufwarnungen gehören in V1.x.
- Datei-/Anhangsverwaltung für Zertifikatsdateien gehört in V2.
- ACME-/PKI-/Deployment-Automation bleibt V3/später.
