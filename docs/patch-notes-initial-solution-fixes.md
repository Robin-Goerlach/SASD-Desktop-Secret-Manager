# Patch Notes – Initial Solution Fixes

Diese kleine Korrekturrunde behebt drei Startprobleme des ersten ZIP-Pakets:

1. In den Testdateien wurde `using Xunit;` ergänzt, damit `[Fact]` und `Assert` aufgelöst werden.
2. In `Program.cs` wird `System.Windows.Forms.Application.Run(...)` nun voll qualifiziert verwendet, um eine Kollision mit dem Namespace `Sasd.SecretManager.Application` zu vermeiden.
3. In `MainForm.cs` wurde die TreeView-Auswahl null-sicher formuliert, damit die Nullability-Warnung verschwindet.

Danach sollten `dotnet build` und `dotnet test` auf einem Rechner mit .NET SDK 8.0.420 deutlich sauberer durchlaufen.
