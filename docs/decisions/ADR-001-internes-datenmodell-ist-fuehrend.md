# ADR-001: Das interne Datenmodell ist führend

## Status
Angenommen

## Entscheidung
Das Projekt verwendet ein eigenes internes Fachmodell als Primärmodell. Externe Formate, insbesondere Password Safe (`.psafe3`), werden auf dieses Modell gemappt.

## Begründung
Die Anwendung soll mehr verwalten können als klassische Login-Daten, etwa technische Zusatzfelder, Infrastrukturbezug, Gruppen, Tags und mehrere fachliche Eintragstypen.
