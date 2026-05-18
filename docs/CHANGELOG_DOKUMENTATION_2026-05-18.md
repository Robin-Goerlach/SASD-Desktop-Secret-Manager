# Changelog Dokumentation – 2026-05-18

## Anlass

Die Projektunterlagen wurden nach dem fortgeschriebenen Lastenheft, Pflichtenheft, Architekturupdate, der neuen Featuremap, dem Security-/Threat-Model, der aktualisierten Roadmap sowie dem Test- und Abnahmekatalog V1 konsolidiert.

## Wesentliche Änderungen

- Lastenheft, Pflichtenheft und Architektur werden als getrennte führende Dokumente geführt.
- Featuremap, Security-/Threat-Model und Test-/Abnahmekatalog wurden als eigene Dokumentationsbereiche ergänzt.
- HTTPS-/TLS-Zertifikatsverwaltung wurde fachlich und technisch sichtbar aufgenommen.
- ADRs wurden erweitert, insbesondere für Zertifikate, Security-Gates und V1-Abnahme.
- Entwicklungspakete wurden an die neue Roadmap angepasst.
- `.svault` erhielt eine eigene technische Spezifikation als Arbeitsgrundlage.
- Password-Safe-Interop wurde als separates Import-/Export-Konzept dokumentiert.
- Repository-Metadaten und Repository-Strategie wurden aktualisiert.

## Wichtigste neue Scope-Aussagen

- V1 ist weiterhin lokal, offline-first und single-user.
- V1 enthält keine Cloud-Synchronisation, kein Team-Sharing, kein Browser-Autofill und keine mobile App.
- Zertifikatsverwaltung ist in V1 als strukturierter Secret-Typ vorgesehen; Anhänge, Ablaufwarnungen und ACME-/PKI-Automation werden gestuft später behandelt.
- Keine Recovery-Backdoor. Recovery wird nur als späteres explizites Konzept ohne Hintertür betrachtet.
- Ein V1-Release ist erst möglich, wenn Security-Gates und Abnahmekatalog erfüllt sind.

## Empfohlene Anwendung im Repository

```bash
# alten Dokumentationsstand sichern
git checkout -b docs/update-2026-05-18
cp -r docs docs_backup_before_2026-05-18

# neuen docs-Ordner aus ZIP übernehmen
# anschließend prüfen
git status
git diff -- docs
```

## Commit-Vorschlag

```bash
git add docs
git commit -m "Update project documentation for V1 scope and certificate handling"
```
