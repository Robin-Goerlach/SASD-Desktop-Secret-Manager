# DSM-003 – Entry Validation

## Ziel

Einträge sollen fachlich sinnvoll validiert werden, ohne das flexible CustomField-Modell zu zerstören.

## V1-Umfang

- Titel ist Pflicht.
- Typ muss gesetzt sein.
- Sensible Felder müssen korrekt als secret klassifizierbar sein.
- Port-Felder sollen numerisch plausibel sein.
- URL-/E-Mail-/Hostname-Felder sollen einfache Plausibilitätsprüfungen erhalten.
- Zertifikatseinträge validieren Valid From/To, Fingerprint-Format und Pflichtfelder soweit sinnvoll.

## UX

Validierung soll helfen, nicht blockieren. Warnungen sind besser als starre Fehlermeldungen, solange kein Datenverlust oder Sicherheitsproblem droht.

## Tests

- Leerer Titel wird abgelehnt.
- Falscher Port wird markiert.
- Secret CustomFields bleiben secret.
- Zertifikat mit Ablaufdatum vor Startdatum wird gewarnt oder abgelehnt.
