# Development Setup

## Zielstand

- .NET SDK **8.0.420** via `global.json`
- WinForms-Anwendung auf `net8.0-windows`
- Debug-Konsole nur im Debug-Build

## Erste lokale Kommandos

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Sasd.SecretManager.WinForms/Sasd.SecretManager.WinForms.csproj
```

## Hinweis

Die WinForms-Anwendung ist für Windows gedacht. Restore, Build und Ausführung sollten daher auf dem Windows-Entwicklungsrechner mit installiertem .NET 8 SDK erfolgen.
