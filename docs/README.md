# Dokumentenübersicht

Dieses Verzeichnis bündelt die fachlichen, technischen, sicherheitsbezogenen und releasebezogenen Grundlagen des Projekts **SASD Desktop Secret Manager**.

Die Dokumentation wurde am **2026-05-18** an den aktuellen Stand aus Lastenheft, Pflichtenheft, Architekturdokument, Featuremap, Security-/Threat-Model, Roadmap und Test-/Abnahmekatalog angepasst. Neu eingearbeitet ist insbesondere die Verwaltung von **HTTPS-/TLS-Zertifikaten** als eigener fachlicher Secret-Kontext.

> **Wichtiger Statushinweis:** Der Code ist ein aktiver Entwicklungsstand mit lauffähigem WinForms-Prototyp, aber noch kein produktiv freigegebenes Sicherheitsprodukt. Die Dokumente unterscheiden deshalb bewusst zwischen bereits umgesetzt, verbindlich für V1, geplant für V1.x/V2 und bewusst vertagt.

## Empfohlene Lesereihenfolge

1. [`lastenheft/Lastenheft_Desktop_Secret_Manager_SASD.md`](lastenheft/Lastenheft_Desktop_Secret_Manager_SASD.md)  
   Fachliche Anforderungen, Ziele, Abgrenzungen und Versionslogik.

2. [`pflichtenheft/Pflichtenheft_Desktop_Secret_Manager_SASD.md`](pflichtenheft/Pflichtenheft_Desktop_Secret_Manager_SASD.md)  
   Konkrete Umsetzungsspezifikation und Liefergrenzen.

3. [`architektur/Architekturdokument_SASD_Desktop_Secret_Manager.md`](architektur/Architekturdokument_SASD_Desktop_Secret_Manager.md)  
   Schichtenmodell, Datenmodell, Speicher-/Security-Architektur und Konsequenzen aus den neuen Anforderungen.

4. [`featuremap/Featuremap_SASD_Desktop_Secret_Manager.md`](featuremap/Featuremap_SASD_Desktop_Secret_Manager.md)  
   Featurebereiche, Versionszuordnung und Priorisierung.

5. [`roadmap.md`](roadmap.md)  
   Praktische Entwicklungsreihenfolge von Milestone 12 bis V1, V1.x und später.

6. [`security/Security_Threat_Model_SASD_Desktop_Secret_Manager.md`](security/Security_Threat_Model_SASD_Desktop_Secret_Manager.md)  
   Schutzmodell, Bedrohungen, Risiken, Kontrollen und V1-Security-Gates.

7. [`tests/Test_und_Abnahmekatalog_V1_SASD_Desktop_Secret_Manager.md`](tests/Test_und_Abnahmekatalog_V1_SASD_Desktop_Secret_Manager.md)  
   Prüfkatalog für automatisierte Tests, manuelle QA und V1-Abnahme.

## Dokumentenstruktur

| Bereich | Zweck |
|---|---|
| `lastenheft/` | Fachliche Sicht: Was soll das Produkt leisten und warum? |
| `pflichtenheft/` | Umsetzungssicht: Wie wird der fachliche Bedarf realisiert? |
| `architektur/` | Technische Struktur: Schichten, Datenmodell, Laufzeit- und Speicherentscheidungen. |
| `featuremap/` | Feature-Landkarte mit V1/V1.x/V2/V3-Zuordnung. |
| `security/` | Security-/Threat-Model, Security-Gates und Sicherheitschecklisten. |
| `tests/` | Teststrategie, Testfälle und Abnahmekriterien für V1. |
| `technical/` | Technische Detailkonzepte, insbesondere `.svault`. |
| `import-export/` | Import-/Export- und Interop-Konzepte, vor allem Password Safe. |
| `ui/` | UI-/UX-Konzepte für die Bedienung und Secret-Typen. |
| `decisions/` | Architekturentscheidungen im ADR-Stil. |
| `development/` | Entwicklungspakete und umsetzungsnahe Mini-Spezifikationen. |
| `release/` | Release-bezogene Begleitdokumente und Freigabehinweise. |

## Zentrale aktuelle Entscheidungen

- Das Produkt bleibt **lokal und offline-first**.
- Das interne Modell bleibt führend; Password Safe ist eine **Interop-Schicht**, nicht das Primärmodell.
- Das Primärformat ist ein eigenes verschlüsseltes **`.svault`**-Format.
- Mehrere unabhängige Tresore sind Teil des Zielbilds.
- Es gibt **keine versteckte Recovery-Backdoor**.
- HTTPS-/TLS-Zertifikate werden als eigener Secret-Typ unterstützt, aber private Schlüssel, PFX-Inhalte und Passphrasen werden wie hochkritische Secrets behandelt.
- V1 konzentriert sich auf lokale Alltagstauglichkeit, Sicherheit, Datenverlustschutz und kontrollierten Import; Cloud, Team-Sharing, Browser-Autofill und PKI-Automation bleiben vertagt.

## Hinweise zur Ablage

Die `.docx`- und `.pdf`-Dateien dienen als gut lesbare Arbeits- und Präsentationsfassungen. Die `.md`-Dateien sind die repositoryfreundlichen Quellen für schnelle Reviews, Diffs und spätere Pull Requests.

Die früheren kombinierten Dokumente aus Lastenheft und Architektur wurden fachlich durch getrennte aktuelle Dokumente ersetzt. Für neue Arbeit sollte daher nicht mehr das alte kombinierte Architektur-/Lastenheftdokument als führend verwendet werden.
