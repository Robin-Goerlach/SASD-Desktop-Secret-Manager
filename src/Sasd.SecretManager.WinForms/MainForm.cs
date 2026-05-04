using Sasd.SecretManager.Application;
using Sasd.SecretManager.Domain;
using Sasd.SecretManager.Security;

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Erste Hauptoberfläche der Anwendung.
/// Die Form bildet bewusst nur ein ruhiges Startgerüst ab.
/// </summary>
public sealed class MainForm : Form
{
    private readonly TreeView _groupTreeView;
    private readonly ListView _entryListView;
    private readonly PropertyGrid _detailsPropertyGrid;
    private readonly ToolStripStatusLabel _statusLabel;

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

        // Eine ruhige dunkle Basis passt gut zur bisher gewünschten visuellen Richtung.
        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;

        DevLog.WriteLine("MainForm wird aufgebaut.");

        var menuStrip = BuildMenuStrip();
        var statusStrip = BuildStatusStrip();
        _statusLabel = new ToolStripStatusLabel("Bereit. Erste UI-Shell geladen.");
        statusStrip.Items.Add(_statusLabel);

        _groupTreeView = BuildGroupTreeView();
        _entryListView = BuildEntryListView();
        _detailsPropertyGrid = BuildDetailsGrid();

        var horizontalSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 290,
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

        horizontalSplit.Panel1.Controls.Add(WrapInPanel("Tresore, Gruppen & Tags", _groupTreeView));
        horizontalSplit.Panel2.Controls.Add(rightSplit);
        rightSplit.Panel1.Controls.Add(WrapInPanel("Einträge", BuildEntryArea()));
        rightSplit.Panel2.Controls.Add(WrapInPanel("Details", _detailsPropertyGrid));

        Controls.Add(horizontalSplit);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;

        SeedDemoData();
        ApplyVaultSummary();
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
        fileMenu.DropDownItems.Add("Neuer Tresor", null, (_, _) => ShowInfo("Noch nicht implementiert."));
        fileMenu.DropDownItems.Add("Tresor öffnen", null, (_, _) => ShowInfo("Noch nicht implementiert."));
        fileMenu.DropDownItems.Add("Tresor speichern", null, (_, _) => ShowInfo("Noch nicht implementiert."));
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Beenden", null, (_, _) => Close());

        var toolsMenu = new ToolStripMenuItem("Werkzeuge");
        toolsMenu.DropDownItems.Add("Passwortgenerator", null, (_, _) => ShowInfo("Noch nicht implementiert."));
        toolsMenu.DropDownItems.Add("Password-Safe-Import", null, (_, _) => ShowInfo("Noch nicht implementiert."));

        var helpMenu = new ToolStripMenuItem("Hilfe");
        helpMenu.DropDownItems.Add("Über", null, (_, _) => ShowInfo("SASD Secret Manager – frühe UI-Shell auf .NET 8."));

        menuStrip.Items.Add(fileMenu);
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
            // Der Node sollte bei AfterSelect in der Praxis vorhanden sein.
            // Dennoch gehen wir hier null-sicher vor, um Warnungen zu vermeiden
            // und die Absicht im Code klar zu machen.
            var selectedText = e.Node?.Text ?? "<kein Knoten>";

            _statusLabel.Text = $"Ausgewählt: {selectedText}";
            DevLog.WriteLine($"TreeView-Auswahl geändert: {selectedText}");
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
        listView.Columns.Add("Tags", 220);

        listView.SelectedIndexChanged += (_, _) =>
        {
            if (listView.SelectedItems.Count == 0)
            {
                return;
            }

            _detailsPropertyGrid.SelectedObject = listView.SelectedItems[0].Tag;
            _statusLabel.Text = $"Eintrag ausgewählt: {listView.SelectedItems[0].Text}";
            DevLog.WriteLine($"ListView-Auswahl geändert: {listView.SelectedItems[0].Text}");
        };

        return listView;
    }

    private PropertyGrid BuildDetailsGrid() => new()
    {
        Dock = DockStyle.Fill,
        HelpVisible = true,
        ToolbarVisible = false,
        BackColor = Color.FromArgb(32, 39, 49),
        ForeColor = Color.Gainsboro,
        ViewBackColor = Color.FromArgb(32, 39, 49),
        ViewForeColor = Color.Gainsboro,
    };

    private Control BuildEntryArea()
    {
        var container = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = BackColor,
        };

        container.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        container.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var searchBox = new TextBox
        {
            Dock = DockStyle.Top,
            PlaceholderText = "Suche nach Titel, Tags, Gruppen oder Zusatzfeldern …",
            Margin = new Padding(0, 0, 0, 12),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };

        searchBox.TextChanged += (_, _) =>
        {
            _statusLabel.Text = string.IsNullOrWhiteSpace(searchBox.Text)
                ? "Bereit. Erste UI-Shell geladen."
                : $"Suche: {searchBox.Text}";
        };

        container.Controls.Add(searchBox, 0, 0);
        container.Controls.Add(_entryListView, 0, 1);
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

    private void SeedDemoData()
    {
        var root = new TreeNode("SASD-GmbH");
        root.Nodes.Add("IONOS");
        root.Nodes.Add("GitHub");
        root.Nodes.Add("Mail");
        root.Nodes.Add("Datenbanken");

        var privateRoot = new TreeNode("Privat");
        privateRoot.Nodes.Add("Allgemein");
        privateRoot.Nodes.Add("Finanzen");

        _groupTreeView.Nodes.Add(root);
        _groupTreeView.Nodes.Add(privateRoot);
        _groupTreeView.ExpandAll();

        AddEntry(new SecretEntry
        {
            Title = "IONOS Webspace FTP",
            EntryType = EntryType.Ftp,
            UserName = "deploy-user",
            Secret = "********",
            Notes = "Früher Demo-Eintrag für die UI-Shell.",
        }, "SASD, IONOS, Deployment");

        AddEntry(new SecretEntry
        {
            Title = "SASD CMS Produktionsdatenbank",
            EntryType = EntryType.Database,
            UserName = "cms_prod",
            Secret = "********",
            Notes = "Später mit Host, Port und DB-Name als Zusatzfelder.",
        }, "SASD, IONOS, MySQL, Produktion");

        AddEntry(new SecretEntry
        {
            Title = "GitHub Organisation",
            EntryType = EntryType.Login,
            UserName = "Robin-Goerlach",
            Secret = "********",
            Notes = "Persönliches Entwicklungs-Repository für frühe Stände.",
        }, "GitHub, SASD");
    }

    private void AddEntry(SecretEntry entry, string tags)
    {
        var item = new ListViewItem(entry.Title);
        item.SubItems.Add(entry.EntryType.ToString());
        item.SubItems.Add(entry.UserName);
        item.SubItems.Add(tags);
        item.Tag = entry;
        _entryListView.Items.Add(item);
    }

    private void ApplyVaultSummary()
    {
        var vault = new SecretVault { Name = "UI-Demo-Tresor" };
        vault.Groups.Add(new EntryGroup { Name = "SASD-GmbH", Path = "SASD-GmbH" });
        vault.Groups.Add(new EntryGroup { Name = "Privat", Path = "Privat" });
        vault.KnownTags.AddRange(new[] { "SASD", "IONOS", "GitHub" });
        vault.Entries.AddRange(_entryListView.Items.Cast<ListViewItem>().Select(item => (SecretEntry)item.Tag!));

        var summaryService = new VaultSummaryService();
        _statusLabel.Text = summaryService.CreateSummary(vault);
    }

    private void ShowInfo(string message)
    {
        DevLog.WriteLine(message);
        MessageBox.Show(this, message, "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
