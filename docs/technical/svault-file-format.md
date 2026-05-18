# `.svault`-Dateiformat – technische Spezifikation

## Zweck

`.svault` ist das interne Primärformat des SASD Desktop Secret Managers. Es ist nicht nur eine beliebige Exportdatei, sondern die führende Speicherform für Tresore.

## Grundprinzip

Ein `.svault`-Container besteht aus einem nicht geheimen Header mit Format- und Kryptografieparametern sowie einer verschlüsselten Nutzlast. Die Nutzlast enthält das serialisierte interne Tresormodell.

## Logische Struktur

```text
.svault
├── header
│   ├── magic
│   ├── formatVersion
│   ├── createdBy
│   ├── keyDerivationProfile
│   ├── encryptionProfile
│   └── payloadContentType
├── kdf
│   ├── algorithm
│   ├── salt
│   ├── iterations oder memory/time/parallelism parameters
│   └── parameterVersion
├── crypto
│   ├── cipher
│   ├── nonce
│   ├── authenticationTag
│   └── keySize
├── metadata
│   ├── savedUtc
│   ├── vaultModelVersion
│   └── optional migration notes
└── encryptedPayload
```

## Nicht geheime Headerdaten

Der Header darf technische Informationen enthalten, die zum Öffnen und Migrieren nötig sind. Er darf keine Nutzdaten, keine Secrets, keine Passwörter, keine Zertifikats-Private-Keys und keine vertraulichen Notizen enthalten.

## Verschlüsselte Nutzlast

Die Nutzlast enthält insbesondere:

- Vault-Metadaten,
- Gruppen,
- Einträge,
- Tags,
- Zusatzfelder,
- Zertifikatseinträge,
- sensible und nicht sensible Feldwerte,
- Zeitstempel,
- spätere Migrationsinformationen.

## Kryptografische Leitlinie

Das aktuelle Architektur- und Codeumfeld arbeitet konservativ mit PBKDF2-SHA256 und AES-256-GCM. Die Dokumentation sieht eine migrationsfähige Struktur vor, damit spätere KDF-Verbesserungen möglich bleiben. KDF-Parameter müssen pro Tresor im Container abgelegt werden und dürfen nicht ausschließlich hart im Code stehen.

## Atomisches Speichern

Speichern erfolgt nicht direkt auf die Zieldatei, sondern kontrolliert:

1. Nutzlast serialisieren.
2. Schlüssel ableiten.
3. Nutzlast verschlüsseln.
4. Container in temporäre Datei schreiben.
5. Datei vollständig schreiben und schließen.
6. vorhandene Zieldatei optional sichern.
7. temporäre Datei kontrolliert an Zielposition verschieben/ersetzen.

## Backup-Regeln

Backups dürfen keine unverschlüsselten Nutzdaten enthalten. Eine `.bak`- oder `.bck`-Datei ist selbst wieder ein vollständiger verschlüsselter Container oder eine verschlüsselte Kopie der vorherigen Tresordatei.

## Fehlerfälle

Die Anwendung muss klar unterscheiden:

- falsches Passwort,
- beschädigter Container,
- unbekannte Formatversion,
- nicht unterstütztes KDF-/Encryption-Profil,
- Dateizugriffsproblem,
- unvollständiger Schreibstand.

Keine dieser Situationen darf als erfolgreicher Ladevorgang erscheinen.

## Migrationen

Das Format muss versioniert bleiben. Neue Features wie Zertifikatsanhänge, TOTP oder Passwort-Historie dürfen nicht durch stillschweigende Strukturänderungen eingeführt werden. Jede Formatänderung benötigt:

- Formatversionsprüfung,
- Migrationspfad,
- Backup vor Migration,
- Negativtests,
- Dokumentation.

## V1-Abnahmerelevanz

Für V1 müssen Speichern, Laden, falsches Passwort, beschädigte Datei, Backup und Migrationserkennung getestet sein. Ohne diese Tests ist ein Release eines Secret Managers nicht verantwortbar.
