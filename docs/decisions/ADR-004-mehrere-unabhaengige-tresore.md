# ADR-004: Mehrere unabhängige Tresore

## Status

Akzeptiert

## Kontext

Private, SASD-interne, kundenbezogene und archivalische Secrets sollen getrennt gehalten werden können.

## Entscheidung

Das Produkt unterstützt mehrere unabhängige Tresore. In V1 ist pro Programmfenster genau ein aktiver schreibbarer Tresor vorgesehen.

## Konsequenzen

- Jeder Tresor hat eigenes Master-Passwort und eigene Datei.
- Inhalte verschiedener Tresore werden nicht automatisch vermischt.
- Cross-Vault-Funktionen werden erst später eingeführt.
- Dateikonflikte werden konservativ behandelt.
