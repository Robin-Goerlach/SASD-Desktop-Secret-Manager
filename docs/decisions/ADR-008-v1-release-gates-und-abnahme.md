# ADR-008: V1-Release-Gates und Abnahme

## Status

Akzeptiert

## Kontext

Der Projektstand ist bereits funktionsreich, aber V1 soll erst freigegeben werden, wenn Alltagstauglichkeit, Sicherheit und Datenverlustschutz belastbar sind.

## Entscheidung

V1 benötigt einen dokumentierten Test- und Abnahmekatalog. Die dort definierten Gates sind vor einem Release zu erfüllen.

## Konsequenzen

- `dotnet build` und `dotnet test` müssen grün sein.
- Manuelle Abnahmeszenarien müssen durchgeführt werden.
- Kritische Fehler blockieren den Release.
- Dokumentation muss mit dem Code-Stand konsistent sein.
