# DSM-009 – V1-Test- und Release-Gates

## Ziel

V1 darf erst als rund gelten, wenn Kernfunktionen, Sicherheit und Dokumentation zusammenpassen.

## Release-Gates

- Build grün.
- Tests grün.
- Keine kritischen Security- oder Datenverlustfehler offen.
- Test- und Abnahmekatalog erfüllt.
- README und Dokumentation konsistent.
- Keine echten Secrets im Repository.

## Manuelle Smoke-Abnahme

1. Neuen Tresor anlegen.
2. Login-, Datenbank-, API- und Zertifikatseintrag anlegen.
3. Speichern, schließen, öffnen.
4. Suche und Filter prüfen.
5. Copy, Clipboard-Clear und Auto-Lock prüfen.
6. `.psafe3`-Import mit Testdatei prüfen.
7. Fehlerfälle testen.

## Ergebnis

Ein Release erfolgt nur, wenn alle Muss-Kriterien erfüllt und bekannte Einschränkungen dokumentiert sind.
