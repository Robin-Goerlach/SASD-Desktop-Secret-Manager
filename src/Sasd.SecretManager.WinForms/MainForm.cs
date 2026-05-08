using Sasd.SecretManager.Application;
using Sasd.SecretManager.Domain;
using Sasd.SecretManager.Security;
using Sasd.SecretManager.Storage;

// ============================================================================
// Dateiüberblick:
// Zentrale Hauptoberfläche mit TreeView, Listenansicht, Detailbereich und den wichtigsten Benutzeraktionen.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Hauptoberfläche der Anwendung.
/// Aktueller Stand: Root-aware Organisation, Kontextmenüs und Drag &amp; Drop
/// für Einträge und Gruppen im TreeView.
/// </summary>
public sealed class MainForm : Form
{
    // Linke Baumansicht für Tresor, Hauptgruppen und Untergruppen.
    private readonly TreeView _groupTreeView;
    // Mittlere Ergebnisliste mit den in der aktuellen Sicht passenden Einträgen.
    private readonly ListView _entryListView;
    // Rechte Detailansicht für den aktuell selektierten Eintrag.
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
    private readonly ToolStripMenuItem _lockVaultMenuItem;
    private readonly ToolStripMenuItem _newEntryMenuItem;
    private readonly ToolStripMenuItem _editEntryMenuItem;
    private readonly ToolStripMenuItem _deleteEntryMenuItem;
    private readonly ToolStripMenuItem _moveEntryMenuItem;
    private readonly ToolStripMenuItem _newGroupMenuItem;
    private readonly ToolStripMenuItem _newSubGroupMenuItem;
    private readonly ToolStripMenuItem _renameGroupMenuItem;
    private readonly ToolStripMenuItem _deleteGroupMenuItem;
    private readonly ContextMenuStrip _groupContextMenu;
    private readonly ContextMenuStrip _entryContextMenu;

    private readonly VaultSummaryService _summaryService = new();
    private readonly VaultUxPolicyService _uxPolicyService = new();
    private readonly VaultQueryService _queryService = new();
    private readonly EntryMutationService _mutationService = new();
    private readonly VaultOrganizationService _organizationService = new();
    private readonly VaultLifecycleService _vaultLifecycleService = new();
    private readonly UnsavedChangesGuardService _unsavedChangesGuardService = new();
    private readonly AutoLockService _autoLockService = new();
    private readonly AutoLockOptions _autoLockOptions = new();
    private readonly IVaultRepository _vaultRepository = new VaultFileRepository();

    // DSM-002:
    // Der WinForms-Timer prueft regelmaessig, ob die Application-Schicht
    // aufgrund der letzten Benutzeraktivitaet eine Sperre empfiehlt.
    private readonly System.Windows.Forms.Timer _autoLockTimer;

    private SecretVault _currentVault = new();
    private string? _currentVaultFilePath;
    private string? _currentMasterPassword;
    private bool _isDirty;
    private bool _isVaultLocked;
    private bool _isAutoLockTimerBusy;
    private DateTimeOffset _lastUserActivityUtc = DateTimeOffset.UtcNow;
    private int _sortColumn;
    private bool _sortAscending = true;
    private bool _closeRequestedFromMenu;

    /// <summary>
    /// Initialisiert die Hauptform.
    /// </summary>
    /// <summary>
    /// Baut Menü, Statusleiste, Navigation, Eintragsliste und Detailbereich auf und verdrahtet alle Kernaktionen.
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

        // Die Debug-Ausgabe hilft dabei, komplexe UI-Abläufe nachzuvollziehen,
        // ohne in produktive Persistenz oder sensible Inhalte einzugreifen.
        DevLog.WriteLine("MainForm wird aufgebaut.");

        _saveVaultMenuItem = new ToolStripMenuItem("Tresor speichern", null, async (_, _) => await SaveVaultAsync(saveAs: false));
        _saveVaultAsMenuItem = new ToolStripMenuItem("Tresor speichern unter", null, async (_, _) => await SaveVaultAsync(saveAs: true));
        _lockVaultMenuItem = new ToolStripMenuItem("Tresor sperren", null, async (_, _) => await ToggleVaultLockAsync());
        _newEntryMenuItem = new ToolStripMenuItem("Neuer Eintrag", null, (_, _) => CreateNewEntry());
        _editEntryMenuItem = new ToolStripMenuItem("Eintrag bearbeiten", null, (_, _) => EditSelectedEntry());
        _deleteEntryMenuItem = new ToolStripMenuItem("Eintrag löschen", null, (_, _) => DeleteSelectedEntry());
        _moveEntryMenuItem = new ToolStripMenuItem("In aktuelle Gruppe verschieben", null, (_, _) => MoveSelectedEntryToCurrentGroup());
        _newGroupMenuItem = new ToolStripMenuItem("Neue Hauptgruppe", null, (_, _) => CreateGroup(createAsRoot: true));
        _newSubGroupMenuItem = new ToolStripMenuItem("Neue Untergruppe", null, (_, _) => CreateSubGroup());
        _renameGroupMenuItem = new ToolStripMenuItem("Gruppe umbenennen", null, (_, _) => RenameSelectedGroup());
        _deleteGroupMenuItem = new ToolStripMenuItem("Gruppe löschen", null, (_, _) => DeleteSelectedGroup());
        _groupContextMenu = BuildGroupContextMenu();
        _entryContextMenu = BuildEntryContextMenu();

        var menuStrip = BuildMenuStrip();
        var statusStrip = BuildStatusStrip();
        _statusLabel = new ToolStripStatusLabel("Bereit. Organisationsfunktionen geladen.");
        statusStrip.Items.Add(_statusLabel);

        _groupTreeView = BuildGroupTreeView();
        _entryListView = BuildEntryListView();
        _detailsPanel = new EntryDetailsPanel();
        _detailsPanel.StatusMessageRequested += message => SetStatus(message);
        _searchTextBox = BuildSearchTextBox();
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

        RegisterActivityTracking(this);
        _autoLockTimer = new System.Windows.Forms.Timer
        {
            Interval = ToTimerIntervalMilliseconds(_autoLockOptions.Normalize().PollInterval),
        };
        _autoLockTimer.Tick += async (_, _) => await HandleAutoLockTimerTickAsync();
        _autoLockTimer.Start();

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
        fileMenu.DropDownItems.Add(_lockVaultMenuItem);
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(_saveVaultMenuItem);
        fileMenu.DropDownItems.Add(_saveVaultAsMenuItem);
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Beenden", null, (_, _) => RequestApplicationClose());

        var entryMenu = new ToolStripMenuItem("Eintrag");
        entryMenu.DropDownItems.Add(_newEntryMenuItem);
        entryMenu.DropDownItems.Add(_editEntryMenuItem);
        entryMenu.DropDownItems.Add(_deleteEntryMenuItem);
        entryMenu.DropDownItems.Add(new ToolStripSeparator());
        entryMenu.DropDownItems.Add(_moveEntryMenuItem);

        var groupMenu = new ToolStripMenuItem("Gruppe");
        groupMenu.DropDownItems.Add(_newGroupMenuItem);
        groupMenu.DropDownItems.Add(_newSubGroupMenuItem);
        groupMenu.DropDownItems.Add(new ToolStripSeparator());
        groupMenu.DropDownItems.Add(_renameGroupMenuItem);
        groupMenu.DropDownItems.Add(_deleteGroupMenuItem);

        var toolsMenu = new ToolStripMenuItem("Werkzeuge");
        toolsMenu.DropDownItems.Add("Passwortgenerator", null, (_, _) => ShowPasswordGenerator());
        toolsMenu.DropDownItems.Add("Password-Safe-Import", null, (_, _) => ShowInfo("Noch nicht implementiert."));

        var helpMenu = new ToolStripMenuItem("Hilfe");
        helpMenu.DropDownItems.Add("Über", null, (_, _) => ShowInfo("SASD Secret Manager – frühe UI-Shell auf .NET 8."));

        menuStrip.Items.Add(fileMenu);
        menuStrip.Items.Add(entryMenu);
        menuStrip.Items.Add(groupMenu);
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

    /// <summary>
    /// Aktualisiert die Statusleiste, wenn eine nicht-leere Meldung vorliegt.
    /// </summary>
    /// <param name="message">Die anzuzeigende Statusmeldung.</param>
    private void SetStatus(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        _statusLabel.Text = message;
    }

    private TreeView BuildGroupTreeView()
    {
        var treeView = new TreeView
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(32, 39, 49),
            ForeColor = Color.Gainsboro,
            BorderStyle = BorderStyle.None,
            HideSelection = false,
            AllowDrop = true,
            ContextMenuStrip = _groupContextMenu,
        };

        treeView.AfterSelect += (_, e) =>
        {
            var selectedPath = e.Node?.Tag as string ?? e.Node?.Text ?? string.Empty;
            DevLog.WriteLine($"TreeView-Auswahl geändert: {selectedPath}");
            ApplyFiltersAndRefresh();
        };

        treeView.NodeMouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView.SelectedNode = e.Node;
            }
        };

        treeView.ItemDrag += (_, e) => BeginGroupDrag(e.Item);
        treeView.DragEnter += (_, e) => HandleTreeDragEnter(e);
        treeView.DragOver += (_, e) => HandleTreeDragOver(treeView, e);
        treeView.DragDrop += (_, e) => HandleTreeDragDrop(treeView, e);
        treeView.DragLeave += (_, _) =>
        {
            if (treeView.SelectedNode is not null && treeView.SelectedNode.Tag is null)
            {
                _statusLabel.Text = "Eintrag auf eine Gruppe ziehen, um ihn zu verschieben.";
            }
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
            ContextMenuStrip = _entryContextMenu,
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
        listView.ItemDrag += (_, e) => BeginEntryDrag(e.Item);
        listView.MouseDown += (_, e) =>
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var item = listView.GetItemAt(e.X, e.Y);
            if (item is not null)
            {
                item.Selected = true;
                item.Focused = true;
            }
        };

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
        var hintLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 44,
            ForeColor = Color.Silver,
            Text = "Tipp: Rechtsklick auf den Tresor oder auf eine Gruppe für Aktionen. Einträge und Gruppen lassen sich per Drag & Drop neu anordnen.",
            Padding = new Padding(0, 0, 0, 8),
        };

        var container = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = BackColor,
        };
        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        container.Controls.Add(hintLabel, 0, 0);
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

        var hintLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            ForeColor = Color.Silver,
            Text = "Weitere Aktionen findest du über die Menüleiste oder per Rechtsklick. Einträge lassen sich per Drag & Drop auf Gruppen verschieben.",
            Padding = new Padding(0, 0, 0, 6),
        };

        var container = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = BackColor,
        };

        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        container.Controls.Add(_searchTextBox, 0, 0);
        container.Controls.Add(actionsPanel, 0, 1);
        container.Controls.Add(hintLabel, 0, 2);
        container.Controls.Add(_entryListView, 0, 3);
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
        _isVaultLocked = false;
        RecordUserActivity();

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

        var rootNode = new TreeNode(string.IsNullOrWhiteSpace(_currentVault.Name) ? "Tresor" : _currentVault.Name)
        {
            Tag = null,
        };

        var groupsByParent = _currentVault.Groups.ToLookup(group => group.ParentGroupId);

        foreach (var rootGroup in groupsByParent[null].OrderBy(group => group.Name, StringComparer.OrdinalIgnoreCase))
        {
            rootNode.Nodes.Add(BuildGroupNodeRecursive(rootGroup, groupsByParent));
        }

        _groupTreeView.Nodes.Add(rootNode);
        rootNode.Expand();
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

    private ContextMenuStrip BuildGroupContextMenu()
    {
        var contextMenu = new ContextMenuStrip();

        var newRootGroupItem = new ToolStripMenuItem("Neue Hauptgruppe", null, (_, _) => CreateGroup(createAsRoot: true));
        var newSubGroupItem = new ToolStripMenuItem("Neue Untergruppe", null, (_, _) => CreateSubGroup());
        var renameGroupItem = new ToolStripMenuItem("Umbenennen", null, (_, _) => RenameSelectedGroup());
        var deleteGroupItem = new ToolStripMenuItem("Löschen", null, (_, _) => DeleteSelectedGroup());

        contextMenu.Items.Add(newRootGroupItem);
        contextMenu.Items.Add(newSubGroupItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(renameGroupItem);
        contextMenu.Items.Add(deleteGroupItem);

        contextMenu.Opening += (_, e) =>
        {
            var hasVault = _currentVault is not null;
            var hasSelectedGroup = !string.IsNullOrWhiteSpace(GetSelectedGroupPath());
            newRootGroupItem.Enabled = hasVault;
            newSubGroupItem.Enabled = hasSelectedGroup;
            renameGroupItem.Enabled = hasSelectedGroup;
            deleteGroupItem.Enabled = hasSelectedGroup;
            e.Cancel = !hasVault;
        };

        return contextMenu;
    }

    private ContextMenuStrip BuildEntryContextMenu()
    {
        var contextMenu = new ContextMenuStrip();

        var newEntryItem = new ToolStripMenuItem("Neuer Eintrag", null, (_, _) => CreateNewEntry());
        var editEntryItem = new ToolStripMenuItem("Bearbeiten", null, (_, _) => EditSelectedEntry());
        var deleteEntryItem = new ToolStripMenuItem("Löschen", null, (_, _) => DeleteSelectedEntry());
        var moveEntryItem = new ToolStripMenuItem("In aktuelle Gruppe verschieben", null, (_, _) => MoveSelectedEntryToCurrentGroup());

        contextMenu.Items.Add(newEntryItem);
        contextMenu.Items.Add(editEntryItem);
        contextMenu.Items.Add(deleteEntryItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(moveEntryItem);

        contextMenu.Opening += (_, e) =>
        {
            var hasSelectedEntry = _entryListView.SelectedItems.Count > 0;
            var hasSelectedGroup = !string.IsNullOrWhiteSpace(GetSelectedGroupPath());
            newEntryItem.Enabled = _currentVault is not null;
            editEntryItem.Enabled = hasSelectedEntry;
            deleteEntryItem.Enabled = hasSelectedEntry;
            moveEntryItem.Enabled = hasSelectedEntry && hasSelectedGroup;
        };

        return contextMenu;
    }

    private void ApplyVaultSummary()
    {
        _statusLabel.Text = _summaryService.CreateSummary(_currentVault);
    }

    private void ApplyFiltersAndRefresh(Guid? preferredSelectionId = null)
    {
        if (_isVaultLocked)
        {
            ShowLockedView();
            return;
        }

        var selectedGroupPath = GetSelectedGroupPath();
        var searchText = _searchTextBox.Text;

        var selectedEntryId = preferredSelectionId
            ?? (_entryListView.SelectedItems.Count > 0 ? ((SecretEntry)_entryListView.SelectedItems[0].Tag!).Id : Guid.Empty);

        var visibleEntries = _queryService.GetVisibleEntries(_currentVault, selectedGroupPath, searchText, _sortColumn, _sortAscending);
        var allowAutoSelection = _uxPolicyService.ShouldAutoSelectFirstEntry(selectedGroupPath, searchText, visibleEntries.Count);
        RefreshEntryList(visibleEntries, selectedEntryId, allowAutoSelection);

        _statusLabel.Text = _uxPolicyService.BuildSelectionStatusText(
            _currentVault,
            selectedGroupPath,
            visibleEntries.Count,
            _sortColumn,
            _sortAscending,
            searchText,
            _currentVaultFilePath,
            _isDirty);

        UpdateUiState();
    }

    private void RefreshEntryList(IReadOnlyList<SecretEntry> entries, Guid selectedEntryId, bool allowAutoSelection)
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

        if (allowAutoSelection)
        {
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
        }
        else
        {
            _entryListView.SelectedItems.Clear();
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

    private const string GroupPathDataFormat = "Sasd.SecretManager.GroupPath";

    private void BeginEntryDrag(object? draggedItem)
    {
        if (draggedItem is not ListViewItem item || item.Tag is not SecretEntry entry)
        {
            return;
        }

        _statusLabel.Text = $"Eintrag wird verschoben: {entry.Title}";
        DevLog.WriteLine($"Drag & Drop gestartet: {entry.Title}");

        var data = new DataObject(typeof(SecretEntry).FullName!, entry);
        _entryListView.DoDragDrop(data, DragDropEffects.Move);
    }

    private void BeginGroupDrag(object? draggedItem)
    {
        if (draggedItem is not TreeNode node || node.Tag is not string sourceGroupPath || string.IsNullOrWhiteSpace(sourceGroupPath))
        {
            return;
        }

        _statusLabel.Text = $"Gruppe wird verschoben: {sourceGroupPath}";
        DevLog.WriteLine($"Gruppen-Drag & Drop gestartet: {sourceGroupPath}");

        var data = new DataObject(GroupPathDataFormat, sourceGroupPath);
        _groupTreeView.DoDragDrop(data, DragDropEffects.Move);
    }

    private void HandleTreeDragEnter(DragEventArgs e)
    {
        e.Effect = TryGetDraggedEntry(e.Data, out _) || TryGetDraggedGroupPath(e.Data, out _)
            ? DragDropEffects.Move
            : DragDropEffects.None;
    }

    private void HandleTreeDragOver(TreeView treeView, DragEventArgs e)
    {
        var targetNode = GetTargetNode(treeView, e);

        if (TryGetDraggedEntry(e.Data, out _))
        {
            if (targetNode?.Tag is not string targetGroupPath || string.IsNullOrWhiteSpace(targetGroupPath))
            {
                e.Effect = DragDropEffects.None;
                _statusLabel.Text = "Einträge können nur auf vorhandene Gruppen verschoben werden.";
                return;
            }

            treeView.SelectedNode = targetNode;
            e.Effect = DragDropEffects.Move;
            _statusLabel.Text = $"Zielgruppe für Eintrag: {targetGroupPath}";
            return;
        }

        if (TryGetDraggedGroupPath(e.Data, out var sourceGroupPath))
        {
            if (!IsValidGroupDropTarget(targetNode, sourceGroupPath))
            {
                e.Effect = DragDropEffects.None;
                _statusLabel.Text = "Gruppen können nicht auf sich selbst oder in eigene Untergruppen verschoben werden.";
                return;
            }

            if (targetNode is not null)
            {
                treeView.SelectedNode = targetNode;
            }

            var targetLabel = targetNode?.Tag as string ?? (targetNode?.Text ?? "oberste Ebene");
            e.Effect = DragDropEffects.Move;
            _statusLabel.Text = $"Ziel für Gruppe: {targetLabel}";
            return;
        }

        e.Effect = DragDropEffects.None;
    }

    private void HandleTreeDragDrop(TreeView treeView, DragEventArgs e)
    {
        var targetNode = GetTargetNode(treeView, e);

        if (TryGetDraggedEntry(e.Data, out var entry))
        {
            if (targetNode?.Tag is not string targetGroupPath || string.IsNullOrWhiteSpace(targetGroupPath))
            {
                ShowInfo("Bitte den Eintrag auf eine vorhandene Gruppe ziehen.");
                return;
            }

            var changed = _organizationService.MoveEntryToGroup(_currentVault, entry, targetGroupPath);
            if (!changed)
            {
                _statusLabel.Text = $"Eintrag bereits in Gruppe: {targetGroupPath}";
                return;
            }

            DevLog.WriteLine($"Eintrag per Drag & Drop verschoben: {entry.Title} -> {targetGroupPath}");
            MarkDirty();
            SelectGroupPath(targetGroupPath);
            ApplyFiltersAndRefresh(entry.Id);
            return;
        }

        if (TryGetDraggedGroupPath(e.Data, out var sourceGroupPath))
        {
            if (!IsValidGroupDropTarget(targetNode, sourceGroupPath))
            {
                ShowInfo("Die gewählte Zielposition ist für diese Gruppe ungültig.");
                return;
            }

            var targetParentPath = targetNode?.Tag as string;

            try
            {
                var targetLabel = string.IsNullOrWhiteSpace(targetParentPath) ? "oberste Ebene" : targetParentPath;
                var moveImpact = _uxPolicyService.GetGroupMoveImpact(_currentVault, sourceGroupPath);

                if (_uxPolicyService.ShouldConfirmGroupMove(moveImpact.ChildGroupCount, moveImpact.EntryCount))
                {
                    var message = _uxPolicyService.BuildGroupMoveConfirmationMessage(
                        sourceGroupPath,
                        targetLabel,
                        moveImpact.ChildGroupCount,
                        moveImpact.EntryCount);

                    var confirmResult = MessageBox.Show(
                        this,
                        message,
                        "SASD Secret Manager",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirmResult != DialogResult.Yes)
                    {
                        _statusLabel.Text = "Gruppenverschiebung abgebrochen.";
                        return;
                    }
                }

                var newPath = _organizationService.MoveGroup(_currentVault, sourceGroupPath, targetParentPath);
                if (string.IsNullOrWhiteSpace(newPath))
                {
                    _statusLabel.Text = "Gruppe bereits an dieser Position.";
                    return;
                }

                DevLog.WriteLine($"Gruppe per Drag & Drop verschoben: {sourceGroupPath} -> {targetLabel}");
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
    }

    private static TreeNode? GetTargetNode(TreeView treeView, DragEventArgs e)
    {
        var clientPoint = treeView.PointToClient(new Point(e.X, e.Y));
        return treeView.GetNodeAt(clientPoint);
    }

    private static bool IsValidGroupDropTarget(TreeNode? targetNode, string sourceGroupPath)
    {
        if (targetNode is null)
        {
            return false;
        }

        var targetGroupPath = targetNode.Tag as string;
        if (string.IsNullOrWhiteSpace(targetGroupPath))
        {
            return true;
        }

        if (string.Equals(targetGroupPath, sourceGroupPath, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (targetGroupPath.StartsWith(sourceGroupPath + "/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private static bool TryGetDraggedGroupPath(IDataObject? dataObject, out string groupPath)
    {
        if (dataObject is not null && dataObject.GetDataPresent(GroupPathDataFormat) && dataObject.GetData(GroupPathDataFormat) is string draggedGroupPath)
        {
            groupPath = draggedGroupPath;
            return true;
        }

        groupPath = string.Empty;
        return false;
    }

    private static bool TryGetDraggedEntry(IDataObject? dataObject, out SecretEntry entry)
    {
        var format = typeof(SecretEntry).FullName!;
        if (dataObject is not null && dataObject.GetDataPresent(format) && dataObject.GetData(format) is SecretEntry draggedEntry)
        {
            entry = draggedEntry;
            return true;
        }

        entry = null!;
        return false;
    }

    private void CreateNewEntry()
    {
        if (!EnsureVaultUnlocked())
        {
            return;
        }

        var selectedGroupPath = GetSelectedGroupPath();
        var model = EntryEditModel.CreateNew(selectedGroupPath);
        var availableGroups = _mutationService.GetAvailableGroupPaths(_currentVault);

        using var dialog = new EntryEditDialog("Neuer Eintrag", model, availableGroups);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        SecretEntry newEntry;
        try
        {
            // DSM-003:
            // Die finale fachliche Validierung liegt bewusst im Application-
            // Service. Der Dialog prüft nur Syntax. Hier werden zusätzlich
            // tresorabhängige Regeln wie doppelte Titel und unbekannte Gruppen
            // behandelt.
            newEntry = _mutationService.CreateEntry(_currentVault, dialog.ResultModel);
        }
        catch (EntryValidationException exception)
        {
            ShowValidationError(exception);
            return;
        }

        DevLog.WriteLine($"Neuer Eintrag erstellt: {newEntry.Title}");
        MarkDirty();
        SelectGroupPath(dialog.ResultModel.SelectedGroupPath);
        ApplyFiltersAndRefresh(newEntry.Id);
    }

    private void EditSelectedEntry()
    {
        if (!EnsureVaultUnlocked())
        {
            return;
        }

        if (!TryGetSelectedEntry(out var entry))
        {
            return;
        }

        var groupPath = _queryService.ResolveGroupPath(_currentVault, entry);
        var model = EntryEditModel.FromEntry(entry, groupPath);
        var availableGroups = _mutationService.GetAvailableGroupPaths(_currentVault);

        using var dialog = new EntryEditDialog($"Eintrag bearbeiten: {entry.Title}", model, availableGroups);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        bool changed;
        try
        {
            // DSM-003:
            // Auch beim Bearbeiten muss die Application-Schicht die endgültige
            // Entscheidung treffen, weil sich z. B. durch Gruppenwechsel ein
            // doppelter Titel innerhalb der Zielgruppe ergeben kann.
            changed = _mutationService.UpdateEntry(_currentVault, entry, dialog.ResultModel);
        }
        catch (EntryValidationException exception)
        {
            ShowValidationError(exception);
            return;
        }

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
        if (!EnsureVaultUnlocked())
        {
            return;
        }

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
        if (!EnsureVaultUnlocked())
        {
            return;
        }

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

    private void CreateGroup(bool createAsRoot = false)
    {
        if (!EnsureVaultUnlocked())
        {
            return;
        }

        var parentGroupPath = createAsRoot ? null : GetSelectedGroupPath();
        using var dialog = new GroupNameDialog(
            title: "Neue Gruppe",
            description: string.IsNullOrWhiteSpace(parentGroupPath)
                ? "Neue Hauptgruppe anlegen"
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

    private void CreateSubGroup()
    {
        if (!EnsureVaultUnlocked())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(GetSelectedGroupPath()))
        {
            ShowInfo("Bitte zuerst die Zielgruppe auswählen, unter der die neue Untergruppe angelegt werden soll.");
            return;
        }

        CreateGroup(createAsRoot: false);
    }

    private void RenameSelectedGroup()
    {
        if (!EnsureVaultUnlocked())
        {
            return;
        }

        var selectedGroupPath = GetSelectedGroupPath();
        if (string.IsNullOrWhiteSpace(selectedGroupPath))
        {
            ShowInfo("Bitte zuerst eine Gruppe auswählen. Der Tresorknoten selbst kann nicht gelöscht werden.");
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
        if (!EnsureVaultUnlocked())
        {
            return;
        }

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
        if (!await ConfirmDiscardOrSaveIfDirtyAsync(UnsavedChangesNavigationAction.CreateNewVault))
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
        if (!await ConfirmDiscardOrSaveIfDirtyAsync(UnsavedChangesNavigationAction.OpenVault))
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
        if (!EnsureVaultUnlocked())
        {
            return false;
        }

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

    /// <summary>
    /// Fragt vor einer riskanten Navigation nach, ob ungespeicherte Änderungen gespeichert werden sollen.
    /// </summary>
    /// <param name="action">Die aktuell angeforderte Aktion, zum Beispiel Neu, Öffnen oder Beenden.</param>
    /// <returns><see langword="true"/>, wenn die Aktion fortgesetzt werden darf; andernfalls <see langword="false"/>.</returns>
    private async Task<bool> ConfirmDiscardOrSaveIfDirtyAsync(UnsavedChangesNavigationAction action)
    {
        if (!_unsavedChangesGuardService.RequiresConfirmation(_isDirty))
        {
            return true;
        }

        var message = _unsavedChangesGuardService.BuildConfirmationMessage(_currentVault.Name, action);
        var result = MessageBox.Show(
            this,
            message,
            "SASD Secret Manager",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        var choice = result switch
        {
            DialogResult.Yes => UnsavedChangesPromptChoice.Save,
            DialogResult.No => UnsavedChangesPromptChoice.Discard,
            _ => UnsavedChangesPromptChoice.Cancel,
        };

        var decision = _unsavedChangesGuardService.Evaluate(_isDirty, choice);
        if (decision == UnsavedChangesGuardDecision.Cancel)
        {
            SetStatus("Aktion abgebrochen. Ungespeicherte Änderungen bleiben erhalten.");
            return false;
        }

        if (decision == UnsavedChangesGuardDecision.SaveBeforeContinuing)
        {
            var saved = await SaveVaultAsync(saveAs: false);
            if (!saved)
            {
                SetStatus("Speichern abgebrochen. Der aktuelle Tresor bleibt geöffnet.");
            }

            return saved;
        }

        SetStatus("Fortfahren ohne Speichern bestätigt.");
        return true;
    }

    /// <summary>
    /// Merkt den expliziten Beenden-Wunsch aus dem Menü vor und löst das normale Schließen der Form aus.
    /// </summary>
    private void RequestApplicationClose()
    {
        _closeRequestedFromMenu = true;
        Close();
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
        var lockMarker = _isVaultLocked ? " 🔒" : string.Empty;
        Text = $"SASD Secret Manager – {vaultName} [{fileLabel}]{dirtyMarker}{lockMarker}";
    }

    private void UpdateUiState()
    {
        var hasVault = _currentVault is not null;
        var canUseVaultContent = hasVault && !_isVaultLocked;
        var hasSelectedEntry = canUseVaultContent && _entryListView.SelectedItems.Count > 0;
        var hasSelectedGroup = canUseVaultContent && !string.IsNullOrWhiteSpace(GetSelectedGroupPath());

        _saveVaultMenuItem.Enabled = canUseVaultContent && _isDirty;
        _saveVaultAsMenuItem.Enabled = canUseVaultContent;
        _lockVaultMenuItem.Enabled = hasVault;
        _lockVaultMenuItem.Text = _isVaultLocked ? "Tresor entsperren" : "Tresor sperren";

        _newEntryMenuItem.Enabled = canUseVaultContent;
        _editEntryMenuItem.Enabled = hasSelectedEntry;
        _deleteEntryMenuItem.Enabled = hasSelectedEntry;
        _moveEntryMenuItem.Enabled = hasSelectedEntry && hasSelectedGroup;
        _newGroupMenuItem.Enabled = canUseVaultContent;
        _newSubGroupMenuItem.Enabled = hasSelectedGroup;
        _renameGroupMenuItem.Enabled = hasSelectedGroup;
        _deleteGroupMenuItem.Enabled = hasSelectedGroup;

        _searchTextBox.Enabled = canUseVaultContent;
        _groupTreeView.Enabled = canUseVaultContent;
        _entryListView.Enabled = canUseVaultContent;
        _detailsPanel.Enabled = canUseVaultContent;

        _newEntryButton.Enabled = canUseVaultContent;
        _editEntryButton.Enabled = hasSelectedEntry;
        _deleteEntryButton.Enabled = hasSelectedEntry;
        _moveEntryButton.Enabled = hasSelectedEntry && hasSelectedGroup;
        _newGroupButton.Enabled = canUseVaultContent;
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
            if (_groupTreeView.Nodes.Count > 0)
            {
                _groupTreeView.SelectedNode = _groupTreeView.Nodes[0];
            }
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


    /// <summary>
    /// Registriert einfache Aktivitätsereignisse auf der Form und ihren Kindern.
    /// </summary>
    /// <remarks>
    /// WinForms-Ereignisse bubbelen nicht so komfortabel wie in Web-UIs. Deshalb
    /// hängen wir die Aktivitätsmessung rekursiv an die vorhandenen Controls.
    /// Das reicht für diesen V1-Milestone und kann später durch eine zentrale
    /// Message-Filter-Lösung ersetzt werden.
    /// </remarks>
    private void RegisterActivityTracking(Control control)
    {
        control.MouseMove += (_, _) => RecordUserActivity();
        control.MouseDown += (_, _) => RecordUserActivity();
        control.KeyDown += (_, _) => RecordUserActivity();

        foreach (Control child in control.Controls)
        {
            RegisterActivityTracking(child);
        }
    }

    private void RecordUserActivity()
    {
        _lastUserActivityUtc = DateTimeOffset.UtcNow;
    }

    private static int ToTimerIntervalMilliseconds(TimeSpan pollInterval)
    {
        var milliseconds = pollInterval.TotalMilliseconds;
        if (milliseconds < 1_000)
        {
            return 1_000;
        }

        if (milliseconds > int.MaxValue)
        {
            return int.MaxValue;
        }

        return (int)milliseconds;
    }

    private async Task HandleAutoLockTimerTickAsync()
    {
        if (_isAutoLockTimerBusy)
        {
            return;
        }

        _isAutoLockTimerBusy = true;
        try
        {
            // Wenn ein modaler Dialog offen ist, sperren wir in diesem V1-Milestone
            // nicht mitten in den Bearbeitungsfluss hinein. Das vermeidet Datenverlust
            // und schwer nachvollziehbare UI-Zustände.
            if (System.Windows.Forms.Application.OpenForms.Count > 1)
            {
                return;
            }

            var shouldLock = _autoLockService.ShouldLock(
                _autoLockOptions,
                _lastUserActivityUtc,
                DateTimeOffset.UtcNow,
                hasOpenVault: _currentVault is not null,
                isAlreadyLocked: _isVaultLocked);

            if (shouldLock)
            {
                await LockVaultAsync(VaultLockReason.Inactivity);
            }
        }
        finally
        {
            _isAutoLockTimerBusy = false;
        }
    }

    private async Task ToggleVaultLockAsync()
    {
        if (_isVaultLocked)
        {
            await UnlockVaultAsync();
        }
        else
        {
            await LockVaultAsync(VaultLockReason.Manual);
        }
    }

    private Task LockVaultAsync(VaultLockReason reason)
    {
        if (_isVaultLocked || _currentVault is null)
        {
            return Task.CompletedTask;
        }

        _isVaultLocked = true;

        var reasonText = reason == VaultLockReason.Inactivity
            ? "Auto-Lock nach Inaktivität"
            : "manuelle Sperre";

        DevLog.Info($"Tresor gesperrt ({reasonText}): {_currentVault.Name}");
        ShowLockedView();
        UpdateWindowTitle();
        return Task.CompletedTask;
    }

    private async Task UnlockVaultAsync()
    {
        if (!_isVaultLocked)
        {
            return;
        }

        using var passwordDialog = new MasterPasswordDialog(
            "Tresor entsperren",
            "Bitte Master-Passwort eingeben, um den Tresor wieder freizugeben.");

        if (passwordDialog.ShowDialog(this) != DialogResult.OK)
        {
            SetStatus("Tresor bleibt gesperrt.");
            return;
        }

        // Wenn der Tresor aus einer Datei geladen wurde und keine ungespeicherten
        // Änderungen vorliegen, prüfen wir das Passwort am echten Tresorformat.
        // Das ist aussagekräftiger als ein reiner Stringvergleich.
        if (!string.IsNullOrWhiteSpace(_currentVaultFilePath) && !_isDirty)
        {
            try
            {
                var loadedVault = await _vaultRepository.LoadAsync(_currentVaultFilePath, passwordDialog.Password);
                _isVaultLocked = false;
                SetCurrentVault(loadedVault, _currentVaultFilePath, passwordDialog.Password, isDirty: false);
                DevLog.Info($"Tresor entsperrt: {loadedVault.Name}");
                SetStatus($"Tresor entsperrt: {loadedVault.Name}");
                return;
            }
            catch (VaultStorageException exception)
            {
                ShowError("Tresor konnte nicht entsperrt werden. Bitte Master-Passwort prüfen.", exception);
                return;
            }
        }

        // Bei neuen oder geänderten Tresoren halten wir den aktuellen In-Memory-Stand
        // bewusst fest, damit Auto-Lock keine ungespeicherten Daten verwirft. Wenn
        // bereits ein Master-Passwort bekannt ist, verwenden wir es als einfache
        // Freigabeprüfung. Für Demo-/Altzustände ohne bekanntes Passwort wird der
        // Dialog trotzdem als bewusste Benutzeraktion genutzt.
        if (!string.IsNullOrEmpty(_currentMasterPassword)
            && !string.Equals(_currentMasterPassword, passwordDialog.Password, StringComparison.Ordinal))
        {
            MessageBox.Show(
                this,
                "Das Master-Passwort ist nicht korrekt. Der Tresor bleibt gesperrt.",
                "SASD Secret Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            SetStatus("Entsperren fehlgeschlagen.");
            DevLog.Info("Entsperren fehlgeschlagen: Master-Passwort stimmt nicht überein.");
            return;
        }

        _currentMasterPassword ??= passwordDialog.Password;
        _isVaultLocked = false;
        RecordUserActivity();
        BuildGroupNodesFromVault();
        ApplyFiltersAndRefresh();
        UpdateWindowTitle();
        DevLog.Info($"Tresor entsperrt: {_currentVault.Name}");
        SetStatus($"Tresor entsperrt: {_currentVault.Name}");
    }

    private void ShowLockedView()
    {
        var vaultName = string.IsNullOrWhiteSpace(_currentVault?.Name)
            ? "Tresor"
            : _currentVault.Name;

        _detailsPanel.ClearDetails();

        _entryListView.BeginUpdate();
        _entryListView.Items.Clear();
        _entryListView.EndUpdate();

        _groupTreeView.BeginUpdate();
        _groupTreeView.Nodes.Clear();
        _groupTreeView.Nodes.Add(new TreeNode($"{vaultName} (gesperrt)") { Tag = null });
        _groupTreeView.EndUpdate();

        _statusLabel.Text = "Tresor ist gesperrt. Über Datei → Tresor entsperren kannst du weiterarbeiten.";
        UpdateUiState();
    }

    private bool EnsureVaultUnlocked()
    {
        if (!_isVaultLocked)
        {
            return true;
        }

        SetStatus("Tresor ist gesperrt. Bitte zuerst entsperren.");
        return false;
    }

    private void ShowPasswordGenerator()
    {
        // DSM-001:
        // Der Generator ist auch unabhaengig vom Bearbeiten eines Eintrags erreichbar.
        // In diesem Modus kann der Benutzer ein Passwort erzeugen und kopieren,
        // ohne den aktuellen Tresor zu veraendern.
        using var dialog = new PasswordGeneratorDialog();

        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.GeneratedPassword is not null)
        {
            SetStatus($"Passwortgenerator: Passwort mit {dialog.GeneratedPassword.Length} Zeichen erzeugt.");
        }
    }

    private enum VaultLockReason
    {
        Manual,
        Inactivity,
    }

    private void ShowInfo(string message)
    {
        DevLog.WriteLine(message);
        MessageBox.Show(this, message, "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowValidationError(EntryValidationException exception)
    {
        // DSM-003:
        // Validierungsfehler sind erwartbare Benutzerfehler und keine technischen
        // Abstürze. Deshalb zeigen wir sie als Warnung und loggen nur eine kurze
        // Info-Meldung, statt einen Fehlerdialog mit Stacktrace-Kontext zu bauen.
        DevLog.Info("Eintrag konnte wegen Validierungsfehlern nicht gespeichert werden.");

        var message = "Der Eintrag konnte nicht gespeichert werden:"
            + Environment.NewLine
            + Environment.NewLine
            + string.Join(
                Environment.NewLine,
                exception.Issues.Select(issue => "• " + issue.Message));

        MessageBox.Show(
            this,
            message,
            "SASD Secret Manager",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    private void ShowError(string message, Exception exception)
    {
        DevLog.WriteException(message, exception);
        MessageBox.Show(this, message, "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        var action = _closeRequestedFromMenu
            ? UnsavedChangesNavigationAction.ExitApplication
            : UnsavedChangesNavigationAction.CloseWindow;

        try
        {
            if (!await ConfirmDiscardOrSaveIfDirtyAsync(action))
            {
                e.Cancel = true;
                return;
            }

            _autoLockTimer.Stop();
            base.OnFormClosing(e);
        }
        finally
        {
            _closeRequestedFromMenu = false;
        }
    }
}
