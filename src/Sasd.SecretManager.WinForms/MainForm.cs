using Sasd.SecretManager.Application;
using Sasd.SecretManager.Domain;
using Sasd.SecretManager.Security;
using Sasd.SecretManager.Storage;

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Hauptoberfläche der Anwendung.
/// Milestone 6 ergänzt Gruppenverwaltung, Eintragslöschung
/// und das Verschieben eines Eintrags in die aktuell ausgewählte Gruppe.
/// </summary>
public sealed class MainForm : Form
{
    private readonly TreeView _groupTreeView;
    private readonly ListView _entryListView;
    private readonly EntryDetailsPanel _detailsPanel;
    private readonly ToolStripStatusLabel _statusLabel;
    private readonly TextBox _searchTextBox;
    private readonly Button _newEntryButton;
    private readonly Button _editEntryButton;
    private readonly Button _deleteEntryButton;
    private readonly Button _moveEntryButton;
    private readonly Button _newGroupButton;
    private readonly Button _renameGroupButton;
    private readonly Button _deleteGroupButton;
    private readonly ToolStripMenuItem _saveVaultMenuItem;
    private readonly ToolStripMenuItem _saveVaultAsMenuItem;
    private readonly ToolStripMenuItem _newEntryMenuItem;
    private readonly ToolStripMenuItem _editEntryMenuItem;
    private readonly ToolStripMenuItem _deleteEntryMenuItem;
    private readonly ToolStripMenuItem _moveEntryMenuItem;
    private readonly ToolStripMenuItem _newGroupMenuItem;
    private readonly ToolStripMenuItem _renameGroupMenuItem;
    private readonly ToolStripMenuItem _deleteGroupMenuItem;

    private readonly VaultSummaryService _summaryService = new();
    private readonly VaultQueryService _queryService = new();
    private readonly EntryMutationService _mutationService = new();
    private readonly VaultOrganizationService _organizationService = new();
    private readonly VaultLifecycleService _vaultLifecycleService = new();
    private readonly IVaultRepository _vaultRepository = new VaultFileRepository();

    private SecretVault _currentVault = new();
    private string? _currentVaultFilePath;
    private string? _currentMasterPassword;
    private bool _isDirty;
    private int _sortColumn;
    private bool _sortAscending = true;

    /// <summary>
    /// Initialisiert die Hauptform.
    /// </summary>
    public MainForm()
    {
        Text = "SASD Secret Manager";
        Width = 1440;
        Height = 900;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 720);

        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;

        DevLog.WriteLine("MainForm wird aufgebaut.");

        _saveVaultMenuItem = new ToolStripMenuItem("Tresor speichern", null, async (_, _) => await SaveVaultAsync(saveAs: false));
        _saveVaultAsMenuItem = new ToolStripMenuItem("Tresor speichern unter", null, async (_, _) => await SaveVaultAsync(saveAs: true));
        _newEntryMenuItem = new ToolStripMenuItem("Neuer Eintrag", null, (_, _) => CreateNewEntry());
        _editEntryMenuItem = new ToolStripMenuItem("Eintrag bearbeiten", null, (_, _) => EditSelectedEntry());
        _deleteEntryMenuItem = new ToolStripMenuItem("Eintrag löschen", null, (_, _) => DeleteSelectedEntry());
        _moveEntryMenuItem = new ToolStripMenuItem("In aktuelle Gruppe verschieben", null, (_, _) => MoveSelectedEntryToCurrentGroup());
        _newGroupMenuItem = new ToolStripMenuItem("Neue Gruppe", null, (_, _) => CreateGroup());
        _renameGroupMenuItem = new ToolStripMenuItem("Gruppe umbenennen", null, (_, _) => RenameSelectedGroup());
        _deleteGroupMenuItem = new ToolStripMenuItem("Gruppe löschen", null, (_, _) => DeleteSelectedGroup());

        var menuStrip = BuildMenuStrip();
        var statusStrip = BuildStatusStrip();
        _statusLabel = new ToolStripStatusLabel("Bereit. Organisationsfunktionen geladen.");
        statusStrip.Items.Add(_statusLabel);

        _groupTreeView = BuildGroupTreeView();
        _entryListView = BuildEntryListView();
        _detailsPanel = new EntryDetailsPanel();
        _searchTextBox = BuildSearchTextBox();
        _detailsPanel.TagClicked += tag =>
        {
            _searchTextBox.Text = tag;
            _searchTextBox.Focus();
            _searchTextBox.SelectionStart = _searchTextBox.TextLength;
            DevLog.WriteLine($"Tag-Filter angewendet: {tag}");
        };
        _newEntryButton = BuildActionButton("Neuer Eintrag", (_, _) => CreateNewEntry());
        _editEntryButton = BuildActionButton("Bearbeiten", (_, _) => EditSelectedEntry());
        _deleteEntryButton = BuildActionButton("Löschen", (_, _) => DeleteSelectedEntry());
        _moveEntryButton = BuildActionButton("In Gruppe verschieben", (_, _) => MoveSelectedEntryToCurrentGroup());
        _newGroupButton = BuildActionButton("Neue Gruppe", (_, _) => CreateGroup());
        _renameGroupButton = BuildActionButton("Umbenennen", (_, _) => RenameSelectedGroup());
        _deleteGroupButton = BuildActionButton("Löschen", (_, _) => DeleteSelectedGroup());

        var horizontalSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 320,
            BackColor = BackColor,
            FixedPanel = FixedPanel.Panel1,
        };

        var rightSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 720,
            BackColor = BackColor,
            FixedPanel = FixedPanel.Panel2,
        };

        horizontalSplit.Panel1.Controls.Add(WrapInPanel("Tresore, Gruppen & Tags", BuildGroupArea()));
        horizontalSplit.Panel2.Controls.Add(rightSplit);
        rightSplit.Panel1.Controls.Add(WrapInPanel("Einträge", BuildEntryArea()));
        rightSplit.Panel2.Controls.Add(WrapInPanel("Details", _detailsPanel));

        Controls.Add(horizontalSplit);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;

        LoadDemoVault();
    }

    private MenuStrip BuildMenuStrip()
    {
        var menuStrip = new MenuStrip
        {
            Dock = DockStyle.Top,
            BackColor = Color.FromArgb(18, 22, 29),
            ForeColor = Color.Gainsboro,
        };

        var fileMenu = new ToolStripMenuItem("Datei");
        fileMenu.DropDownItems.Add("Neuer Tresor", null, async (_, _) => await CreateNewVaultAsync());
        fileMenu.DropDownItems.Add("Tresor öffnen", null, async (_, _) => await OpenVaultAsync());
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(_saveVaultMenuItem);
        fileMenu.DropDownItems.Add(_saveVaultAsMenuItem);
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(_newEntryMenuItem);
        fileMenu.DropDownItems.Add(_editEntryMenuItem);
        fileMenu.DropDownItems.Add(_deleteEntryMenuItem);
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Beenden", null, (_, _) => Close());

        var organizeMenu = new ToolStripMenuItem("Organisieren");
        organizeMenu.DropDownItems.Add(_newGroupMenuItem);
        organizeMenu.DropDownItems.Add(_renameGroupMenuItem);
        organizeMenu.DropDownItems.Add(_deleteGroupMenuItem);
        organizeMenu.DropDownItems.Add(new ToolStripSeparator());
        organizeMenu.DropDownItems.Add(_moveEntryMenuItem);

        var toolsMenu = new ToolStripMenuItem("Werkzeuge");
        toolsMenu.DropDownItems.Add("Passwortgenerator", null, (_, _) => ShowInfo("Noch nicht implementiert."));
        toolsMenu.DropDownItems.Add("Password-Safe-Import", null, (_, _) => ShowInfo("Noch nicht implementiert."));

        var helpMenu = new ToolStripMenuItem("Hilfe");
        helpMenu.DropDownItems.Add("Über", null, (_, _) => ShowInfo("SASD Secret Manager – frühe UI-Shell auf .NET 8."));

        menuStrip.Items.Add(fileMenu);
        menuStrip.Items.Add(organizeMenu);
        menuStrip.Items.Add(toolsMenu);
        menuStrip.Items.Add(helpMenu);
        return menuStrip;
    }

    private static StatusStrip BuildStatusStrip() => new()
    {
        Dock = DockStyle.Bottom,
        BackColor = Color.FromArgb(18, 22, 29),
        ForeColor = Color.Gainsboro,
    };

    private TreeView BuildGroupTreeView()
    {
        var treeView = new TreeView
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(32, 39, 49),
            ForeColor = Color.Gainsboro,
            BorderStyle = BorderStyle.None,
            HideSelection = false,
        };

        treeView.AfterSelect += (_, e) =>
        {
            var selectedPath = e.Node?.Tag as string ?? e.Node?.Text ?? string.Empty;
            DevLog.WriteLine($"TreeView-Auswahl geändert: {selectedPath}");
            ApplyFiltersAndRefresh();
        };

        return treeView;
    }

    private ListView BuildEntryListView()
    {
        var listView = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = false,
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(32, 39, 49),
            ForeColor = Color.Gainsboro,
            HideSelection = false,
        };

        listView.Columns.Add("Titel", 280);
        listView.Columns.Add("Typ", 130);
        listView.Columns.Add("Benutzer", 180);
        listView.Columns.Add("Tags", 260);

        listView.SelectedIndexChanged += (_, _) =>
        {
            if (listView.SelectedItems.Count == 0)
            {
                _detailsPanel.ClearDetails();
                UpdateUiState();
                return;
            }

            var entry = (SecretEntry)listView.SelectedItems[0].Tag!;
            ShowEntryDetails(entry);
            _statusLabel.Text = $"Eintrag ausgewählt: {listView.SelectedItems[0].Text}";
            DevLog.WriteLine($"ListView-Auswahl geändert: {listView.SelectedItems[0].Text}");
            UpdateUiState();
        };

        listView.ColumnClick += (_, e) =>
        {
            if (_sortColumn == e.Column)
            {
                _sortAscending = !_sortAscending;
            }
            else
            {
                _sortColumn = e.Column;
                _sortAscending = true;
            }

            DevLog.WriteLine($"Sortierung geändert: Spalte {_sortColumn}, aufsteigend={_sortAscending}");
            ApplyFiltersAndRefresh();
        };

        listView.DoubleClick += (_, _) => EditSelectedEntry();
        return listView;
    }

    private TextBox BuildSearchTextBox()
    {
        var searchBox = new TextBox
        {
            Dock = DockStyle.Top,
            PlaceholderText = "Suche nach Titel, Tags, Gruppen oder Zusatzfeldern …",
            Margin = new Padding(0, 0, 0, 8),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };

        searchBox.TextChanged += (_, _) =>
        {
            DevLog.WriteLine(string.IsNullOrWhiteSpace(searchBox.Text)
                ? "Suche zurückgesetzt."
                : $"Suche geändert: {searchBox.Text}");
            ApplyFiltersAndRefresh();
        };

        return searchBox;
    }

    private Button BuildActionButton(string text, EventHandler onClick)
    {
        var button = new Button
        {
            AutoSize = true,
            Text = text,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(45, 86, 160),
            ForeColor = Color.WhiteSmoke,
            Margin = new Padding(0, 0, 8, 0),
            Padding = new Padding(12, 5, 12, 5),
        };
        button.Click += onClick;
        return button;
    }

    private Control BuildGroupArea()
    {
        var actionsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 0, 0, 8),
            BackColor = BackColor,
        };
        actionsPanel.Controls.Add(_newGroupButton);
        actionsPanel.Controls.Add(_renameGroupButton);
        actionsPanel.Controls.Add(_deleteGroupButton);

        var container = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = BackColor,
        };
        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        container.Controls.Add(actionsPanel, 0, 0);
        container.Controls.Add(_groupTreeView, 0, 1);
        return container;
    }

    private Control BuildEntryArea()
    {
        var actionsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 0, 0, 8),
            BackColor = BackColor,
        };
        actionsPanel.Controls.Add(_newEntryButton);
        actionsPanel.Controls.Add(_editEntryButton);
        actionsPanel.Controls.Add(_deleteEntryButton);
        actionsPanel.Controls.Add(_moveEntryButton);

        var container = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = BackColor,
        };

        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        container.Controls.Add(_searchTextBox, 0, 0);
        container.Controls.Add(actionsPanel, 0, 1);
        container.Controls.Add(_entryListView, 0, 2);
        return container;
    }

    private Control WrapInPanel(string title, Control content)
    {
        var outerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            BackColor = BackColor,
        };

        var titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 34,
            Text = title,
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
            ForeColor = Color.WhiteSmoke,
            Padding = new Padding(0, 0, 0, 8),
        };

        var innerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(32, 39, 49),
            Padding = new Padding(10),
        };

        content.Dock = DockStyle.Fill;
        innerPanel.Controls.Add(content);
        outerPanel.Controls.Add(innerPanel);
        outerPanel.Controls.Add(titleLabel);
        return outerPanel;
    }

    private void LoadDemoVault()
    {
        var factory = new DemoVaultFactory();
        SetCurrentVault(factory.CreateDemoVault(), null, null, isDirty: false);
    }

    private void SetCurrentVault(SecretVault vault, string? filePath, string? masterPassword, bool isDirty)
    {
        _currentVault = vault ?? throw new ArgumentNullException(nameof(vault));
        _currentVaultFilePath = filePath;
        _currentMasterPassword = masterPassword;
        _isDirty = isDirty;

        BuildGroupNodesFromVault();
        ApplyVaultSummary();
        UpdateWindowTitle();
        UpdateUiState();

        if (_groupTreeView.Nodes.Count > 0)
        {
            _groupTreeView.SelectedNode = _groupTreeView.Nodes[0];
        }
        else
        {
            ApplyFiltersAndRefresh();
        }
    }

    private void BuildGroupNodesFromVault()
    {
        _groupTreeView.BeginUpdate();
        _groupTreeView.Nodes.Clear();

        var groupsByParent = _currentVault.Groups.ToLookup(group => group.ParentGroupId);

        foreach (var rootGroup in groupsByParent[null].OrderBy(group => group.Name, StringComparer.OrdinalIgnoreCase))
        {
            _groupTreeView.Nodes.Add(BuildGroupNodeRecursive(rootGroup, groupsByParent));
        }

        _groupTreeView.ExpandAll();
        _groupTreeView.EndUpdate();
    }

    private TreeNode BuildGroupNodeRecursive(EntryGroup group, ILookup<Guid?, EntryGroup> groupsByParent)
    {
        var node = new TreeNode(group.Name)
        {
            Tag = group.Path,
        };

        foreach (var childGroup in groupsByParent[group.Id].OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
        {
            node.Nodes.Add(BuildGroupNodeRecursive(childGroup, groupsByParent));
        }

        return node;
    }

    private void ApplyVaultSummary()
    {
        _statusLabel.Text = _summaryService.CreateSummary(_currentVault);
    }

    private void ApplyFiltersAndRefresh(Guid? preferredSelectionId = null)
    {
        var selectedGroupPath = GetSelectedGroupPath();
        var searchText = _searchTextBox.Text;

        var selectedEntryId = preferredSelectionId
            ?? (_entryListView.SelectedItems.Count > 0 ? ((SecretEntry)_entryListView.SelectedItems[0].Tag!).Id : Guid.Empty);

        var visibleEntries = _queryService.GetVisibleEntries(_currentVault, selectedGroupPath, searchText, _sortColumn, _sortAscending);
        RefreshEntryList(visibleEntries, selectedEntryId);

        var groupLabel = string.IsNullOrWhiteSpace(selectedGroupPath) ? "Alle Gruppen" : selectedGroupPath;
        var searchLabel = string.IsNullOrWhiteSpace(searchText) ? string.Empty : $" · Suche: {searchText}";
        var sortDirection = _sortAscending ? "aufsteigend" : "absteigend";
        var dirtyLabel = _isDirty ? " · ungespeichert" : string.Empty;
        var fileLabel = string.IsNullOrWhiteSpace(_currentVaultFilePath)
            ? " · ohne Datei"
            : $" · Datei: {Path.GetFileName(_currentVaultFilePath)}";
        _statusLabel.Text = $"{groupLabel} · {visibleEntries.Count} Einträge · Sortierung Spalte {_sortColumn + 1} ({sortDirection}){searchLabel}{fileLabel}{dirtyLabel}";
        UpdateUiState();
    }

    private void RefreshEntryList(IReadOnlyList<SecretEntry> entries, Guid selectedEntryId)
    {
        _entryListView.BeginUpdate();
        _entryListView.Items.Clear();

        ListViewItem? itemToSelect = null;

        foreach (var entry in entries)
        {
            var item = new ListViewItem(entry.Title);
            item.SubItems.Add(entry.EntryType.ToString());
            item.SubItems.Add(entry.UserName);
            item.SubItems.Add(entry.Tags.Count == 0 ? string.Empty : string.Join(", ", entry.Tags));
            item.Tag = entry;

            _entryListView.Items.Add(item);

            if (entry.Id == selectedEntryId)
            {
                itemToSelect = item;
            }
        }

        _entryListView.EndUpdate();

        if (itemToSelect is not null)
        {
            itemToSelect.Selected = true;
            itemToSelect.EnsureVisible();
        }
        else if (_entryListView.Items.Count > 0)
        {
            _entryListView.Items[0].Selected = true;
        }
        else
        {
            _detailsPanel.ClearDetails();
        }

        UpdateUiState();
    }

    private void ShowEntryDetails(SecretEntry entry)
    {
        var details = CreateDetailViewModel(entry);
        _detailsPanel.DisplayEntry(details);
    }

    private EntryDetailViewModel CreateDetailViewModel(SecretEntry entry)
    {
        var groupPath = _queryService.ResolveGroupPath(_currentVault, entry);
        return EntryDetailViewModel.FromEntry(entry, groupPath);
    }

    private void CreateNewEntry()
    {
        var selectedGroupPath = GetSelectedGroupPath();
        var model = EntryEditModel.CreateNew(selectedGroupPath);
        var availableGroups = _mutationService.GetAvailableGroupPaths(_currentVault);

        using var dialog = new EntryEditDialog("Neuer Eintrag", model, availableGroups, _currentVault.KnownTags);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var newEntry = _mutationService.CreateEntry(_currentVault, dialog.ResultModel);
        DevLog.WriteLine($"Neuer Eintrag erstellt: {newEntry.Title}");
        MarkDirty();
        SelectGroupPath(dialog.ResultModel.SelectedGroupPath);
        ApplyFiltersAndRefresh(newEntry.Id);
    }

    private void EditSelectedEntry()
    {
        if (!TryGetSelectedEntry(out var entry))
        {
            return;
        }

        var groupPath = _queryService.ResolveGroupPath(_currentVault, entry);
        var model = EntryEditModel.FromEntry(entry, groupPath);
        var availableGroups = _mutationService.GetAvailableGroupPaths(_currentVault);

        using var dialog = new EntryEditDialog($"Eintrag bearbeiten: {entry.Title}", model, availableGroups, _currentVault.KnownTags);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var changed = _mutationService.UpdateEntry(_currentVault, entry, dialog.ResultModel);
        if (!changed)
        {
            DevLog.WriteLine($"Bearbeiten beendet ohne Änderung: {entry.Title}");
            _statusLabel.Text = $"Eintrag unverändert: {entry.Title}";
            ApplyFiltersAndRefresh(entry.Id);
            return;
        }

        DevLog.WriteLine($"Eintrag bearbeitet: {entry.Title}");
        MarkDirty();
        SelectGroupPath(dialog.ResultModel.SelectedGroupPath);
        ApplyFiltersAndRefresh(entry.Id);
    }

    private void DeleteSelectedEntry()
    {
        if (!TryGetSelectedEntry(out var entry))
        {
            return;
        }

        var result = MessageBox.Show(
            this,
            $"Möchtest du den Eintrag '{entry.Title}' wirklich löschen?",
            "SASD Secret Manager",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
        {
            return;
        }

        if (!_organizationService.DeleteEntry(_currentVault, entry))
        {
            ShowInfo("Der ausgewählte Eintrag konnte nicht gelöscht werden.");
            return;
        }

        DevLog.WriteLine($"Eintrag gelöscht: {entry.Title}");
        MarkDirty();
        ApplyFiltersAndRefresh();
    }

    private void MoveSelectedEntryToCurrentGroup()
    {
        if (!TryGetSelectedEntry(out var entry))
        {
            return;
        }

        var targetGroupPath = GetSelectedGroupPath();
        if (string.IsNullOrWhiteSpace(targetGroupPath))
        {
            ShowInfo("Bitte zuerst eine Zielgruppe im TreeView auswählen.");
            return;
        }

        var changed = _organizationService.MoveEntryToGroup(_currentVault, entry, targetGroupPath);
        if (!changed)
        {
            _statusLabel.Text = $"Eintrag bereits in Gruppe: {targetGroupPath}";
            return;
        }

        DevLog.WriteLine($"Eintrag verschoben: {entry.Title} -> {targetGroupPath}");
        MarkDirty();
        SelectGroupPath(targetGroupPath);
        ApplyFiltersAndRefresh(entry.Id);
    }

    private void CreateGroup()
    {
        var parentGroupPath = GetSelectedGroupPath();
        using var dialog = new GroupNameDialog(
            title: "Neue Gruppe",
            description: string.IsNullOrWhiteSpace(parentGroupPath)
                ? "Neue Root-Gruppe anlegen"
                : $"Neue Untergruppe unter:{Environment.NewLine}{parentGroupPath}",
            initialName: string.Empty);

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var group = _organizationService.CreateGroup(_currentVault, dialog.GroupName, parentGroupPath);
            DevLog.WriteLine($"Gruppe erstellt: {group.Path}");
            MarkDirty();
            BuildGroupNodesFromVault();
            SelectGroupPath(group.Path);
            ApplyFiltersAndRefresh();
        }
        catch (InvalidOperationException exception)
        {
            ShowInfo(exception.Message);
        }
    }

    private void RenameSelectedGroup()
    {
        var selectedGroupPath = GetSelectedGroupPath();
        if (string.IsNullOrWhiteSpace(selectedGroupPath))
        {
            ShowInfo("Bitte zuerst eine Gruppe auswählen.");
            return;
        }

        var group = _currentVault.Groups.FirstOrDefault(item => string.Equals(item.Path, selectedGroupPath, StringComparison.OrdinalIgnoreCase));
        if (group is null)
        {
            ShowInfo("Die ausgewählte Gruppe konnte nicht gefunden werden.");
            return;
        }

        using var dialog = new GroupNameDialog(
            title: "Gruppe umbenennen",
            description: $"Aktueller Pfad:{Environment.NewLine}{group.Path}",
            initialName: group.Name);

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var newPath = _organizationService.RenameGroup(_currentVault, selectedGroupPath, dialog.GroupName);
            DevLog.WriteLine($"Gruppe umbenannt: {selectedGroupPath} -> {newPath}");
            MarkDirty();
            BuildGroupNodesFromVault();
            SelectGroupPath(newPath);
            ApplyFiltersAndRefresh();
        }
        catch (InvalidOperationException exception)
        {
            ShowInfo(exception.Message);
        }
    }

    private void DeleteSelectedGroup()
    {
        var selectedGroupPath = GetSelectedGroupPath();
        if (string.IsNullOrWhiteSpace(selectedGroupPath))
        {
            ShowInfo("Bitte zuerst eine Gruppe auswählen.");
            return;
        }

        var group = _currentVault.Groups.FirstOrDefault(item => string.Equals(item.Path, selectedGroupPath, StringComparison.OrdinalIgnoreCase));
        if (group is null)
        {
            ShowInfo("Die ausgewählte Gruppe konnte nicht gefunden werden.");
            return;
        }

        var result = MessageBox.Show(
            this,
            $"Möchtest du die Gruppe '{group.Path}' wirklich löschen?",
            "SASD Secret Manager",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var parentPath = group.ParentGroupId is Guid parentId
            ? _currentVault.Groups.FirstOrDefault(item => item.Id == parentId)?.Path
            : null;

        try
        {
            _organizationService.DeleteGroup(_currentVault, selectedGroupPath);
            DevLog.WriteLine($"Gruppe gelöscht: {selectedGroupPath}");
            MarkDirty();
            BuildGroupNodesFromVault();
            if (!string.IsNullOrWhiteSpace(parentPath))
            {
                SelectGroupPath(parentPath);
            }
            else if (_groupTreeView.Nodes.Count > 0)
            {
                _groupTreeView.SelectedNode = _groupTreeView.Nodes[0];
            }
            ApplyFiltersAndRefresh();
        }
        catch (InvalidOperationException exception)
        {
            ShowInfo(exception.Message);
        }
    }

    private bool TryGetSelectedEntry(out SecretEntry entry)
    {
        if (_entryListView.SelectedItems.Count > 0 && _entryListView.SelectedItems[0].Tag is SecretEntry selectedEntry)
        {
            entry = selectedEntry;
            return true;
        }

        entry = null!;
        ShowInfo("Bitte zuerst einen Eintrag auswählen.");
        return false;
    }

    private string? GetSelectedGroupPath() => _groupTreeView.SelectedNode?.Tag as string;

    private async Task CreateNewVaultAsync()
    {
        if (!await ConfirmDiscardOrSaveIfDirtyAsync())
        {
            return;
        }

        using var dialog = new NewVaultDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var vault = _vaultLifecycleService.CreateNewVault(dialog.VaultName);
        SetCurrentVault(vault, null, dialog.MasterPassword, isDirty: true);
        DevLog.WriteLine($"Neuer Tresor angelegt: {vault.Name}");
        _statusLabel.Text = $"Neuer Tresor angelegt: {vault.Name}";
    }

    private async Task OpenVaultAsync()
    {
        if (!await ConfirmDiscardOrSaveIfDirtyAsync())
        {
            return;
        }

        using var fileDialog = new OpenFileDialog
        {
            Title = "Tresor öffnen",
            Filter = "SASD Vault (*.svault)|*.svault",
            CheckFileExists = true,
            Multiselect = false,
        };

        if (fileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        using var passwordDialog = new MasterPasswordDialog(
            "Tresor öffnen",
            "Bitte Master-Passwort für den ausgewählten Tresor eingeben.");

        if (passwordDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var loadedVault = await _vaultRepository.LoadAsync(fileDialog.FileName, passwordDialog.Password);
            SetCurrentVault(loadedVault, fileDialog.FileName, passwordDialog.Password, isDirty: false);
            DevLog.WriteLine($"Tresor geöffnet: {Path.GetFileName(fileDialog.FileName)}");
            _statusLabel.Text = $"Tresor geöffnet: {Path.GetFileName(fileDialog.FileName)}";
        }
        catch (VaultStorageException exception)
        {
            ShowError(exception.Message, exception);
        }
    }

    private async Task<bool> SaveVaultAsync(bool saveAs)
    {
        var targetPath = _currentVaultFilePath;

        if (saveAs || string.IsNullOrWhiteSpace(targetPath))
        {
            using var fileDialog = new SaveFileDialog
            {
                Title = saveAs ? "Tresor speichern unter" : "Tresor speichern",
                Filter = "SASD Vault (*.svault)|*.svault",
                DefaultExt = "svault",
                AddExtension = true,
                FileName = CreateSuggestedFileName(),
            };

            if (fileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return false;
            }

            targetPath = fileDialog.FileName;
        }

        var password = _currentMasterPassword;
        if (string.IsNullOrWhiteSpace(password))
        {
            using var passwordDialog = new MasterPasswordDialog(
                "Tresor speichern",
                "Bitte Master-Passwort für diesen Tresor eingeben.",
                warnOnWeakPassword: true);

            if (passwordDialog.ShowDialog(this) != DialogResult.OK)
            {
                return false;
            }

            password = passwordDialog.Password;
        }

        try
        {
            await _vaultRepository.SaveAsync(_currentVault, targetPath!, password!);
            _currentVaultFilePath = targetPath;
            _currentMasterPassword = password;
            _isDirty = false;
            UpdateWindowTitle();
            ApplyFiltersAndRefresh();
            DevLog.WriteLine($"Tresor gespeichert: {Path.GetFileName(targetPath)}");
            _statusLabel.Text = $"Tresor gespeichert: {Path.GetFileName(targetPath)}";
            return true;
        }
        catch (VaultStorageException exception)
        {
            ShowError(exception.Message, exception);
            return false;
        }
    }

    private async Task<bool> ConfirmDiscardOrSaveIfDirtyAsync()
    {
        if (!_isDirty)
        {
            return true;
        }

        var result = MessageBox.Show(
            this,
            "Der aktuelle Tresor enthält ungespeicherte Änderungen. Möchtest du ihn vor dem Fortfahren speichern?",
            "SASD Secret Manager",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        if (result == DialogResult.Cancel)
        {
            return false;
        }

        if (result == DialogResult.No)
        {
            return true;
        }

        return await SaveVaultAsync(saveAs: false);
    }

    private void MarkDirty()
    {
        _isDirty = true;
        UpdateWindowTitle();
        ApplyFiltersAndRefresh();
    }

    private void UpdateWindowTitle()
    {
        var vaultName = string.IsNullOrWhiteSpace(_currentVault.Name) ? "Unbenannter Tresor" : _currentVault.Name;
        var fileLabel = string.IsNullOrWhiteSpace(_currentVaultFilePath)
            ? "ohne Datei"
            : Path.GetFileName(_currentVaultFilePath);
        var dirtyMarker = _isDirty ? " *" : string.Empty;
        Text = $"SASD Secret Manager – {vaultName} [{fileLabel}]{dirtyMarker}";
    }

    private void UpdateUiState()
    {
        var hasVault = _currentVault is not null;
        var hasSelectedEntry = _entryListView.SelectedItems.Count > 0;
        var hasSelectedGroup = !string.IsNullOrWhiteSpace(GetSelectedGroupPath());

        _saveVaultMenuItem.Enabled = hasVault && _isDirty;
        _saveVaultAsMenuItem.Enabled = hasVault;
        _newEntryMenuItem.Enabled = hasVault;
        _editEntryMenuItem.Enabled = hasSelectedEntry;
        _deleteEntryMenuItem.Enabled = hasSelectedEntry;
        _moveEntryMenuItem.Enabled = hasSelectedEntry && hasSelectedGroup;
        _newGroupMenuItem.Enabled = hasVault;
        _renameGroupMenuItem.Enabled = hasSelectedGroup;
        _deleteGroupMenuItem.Enabled = hasSelectedGroup;

        _newEntryButton.Enabled = hasVault;
        _editEntryButton.Enabled = hasSelectedEntry;
        _deleteEntryButton.Enabled = hasSelectedEntry;
        _moveEntryButton.Enabled = hasSelectedEntry && hasSelectedGroup;
        _newGroupButton.Enabled = hasVault;
        _renameGroupButton.Enabled = hasSelectedGroup;
        _deleteGroupButton.Enabled = hasSelectedGroup;
    }

    private string CreateSuggestedFileName()
    {
        var name = string.IsNullOrWhiteSpace(_currentVault.Name) ? "secret-vault" : _currentVault.Name.Trim();
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '-');
        }

        return name.EndsWith(".svault", StringComparison.OrdinalIgnoreCase) ? name : name + ".svault";
    }

    private void SelectGroupPath(string? groupPath)
    {
        if (string.IsNullOrWhiteSpace(groupPath))
        {
            return;
        }

        var node = FindNodeByPath(_groupTreeView.Nodes, groupPath);
        if (node is not null)
        {
            _groupTreeView.SelectedNode = node;
            node.EnsureVisible();
        }
    }

    private static TreeNode? FindNodeByPath(TreeNodeCollection nodes, string groupPath)
    {
        foreach (TreeNode node in nodes)
        {
            if (string.Equals(node.Tag as string, groupPath, StringComparison.OrdinalIgnoreCase))
            {
                return node;
            }

            var child = FindNodeByPath(node.Nodes, groupPath);
            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private void ShowInfo(string message)
    {
        DevLog.WriteLine(message);
        MessageBox.Show(this, message, "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowError(string message, Exception exception)
    {
        DevLog.WriteException(message, exception);
        MessageBox.Show(this, message, "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        if (!await ConfirmDiscardOrSaveIfDirtyAsync())
        {
            e.Cancel = true;
            return;
        }

        base.OnFormClosing(e);
    }
}
