# ADR-001: Internes Datenmodell ist führend

## Status

Akzeptiert

## Kontext

Das Produkt soll mehr verwalten als klassische Web-Logins. Es benötigt technische Secret-Typen, Zusatzfelder, Tags, Zertifikatsinformationen und spätere Erweiterungen.

## Entscheidung

Das interne Datenmodell des SASD Desktop Secret Managers ist führend. Externe Formate wie Password Safe werden über Mapping und Interop angebunden, bestimmen aber nicht die Grenzen des Produkts.

## Konsequenzen

- `.svault` bleibt Primärformat.
- `.psafe3` wird importiert, aber nicht zum internen Arbeitsformat gemacht.
- Nicht abbildbare fremde Inhalte werden dokumentiert statt stillschweigend verworfen.
- Neue interne Features wie Zertifikate dürfen nicht an Fremdformatgrenzen scheitern.
