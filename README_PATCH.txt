Milestone 2 Patch: UI shell bound to in-memory vault data

Contains:
- demo vault factory with realistic sample groups and entries
- query service for group filtering, search and sorting
- detail view model with masked secret preview
- WinForms update for live filtering, sorting and double-click details dialog
- new application unit test project and solution update

Apply:
1. Unzip this patch into the repository root.
2. Overwrite files when asked.
3. Run:
   dotnet restore
   dotnet build
   dotnet test
   dotnet run --project src/Sasd.SecretManager.WinForms/Sasd.SecretManager.WinForms.csproj

Suggested commit message:
Bind initial UI shell to in-memory vault data and selection logic
