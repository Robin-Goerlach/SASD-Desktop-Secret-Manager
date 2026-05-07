using Sasd.SecretManager.Application;
using Sasd.SecretManager.Security;
using UiTimer = System.Windows.Forms.Timer;

// ============================================================================
// Dateiüberblick:
// Rechte Detailansicht der Hauptoberfläche mit Copy-Aktionen und Tag-Interaktion.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Strukturierter Detailbereich für die rechte Seite der Hauptansicht.
/// </summary>
public sealed class EntryDetailsPanel : UserControl
{
    private readonly Label _titleLabel;
    private readonly Label _typeLabel;
    private readonly FlowLayoutPanel _tagFlowPanel;
    private readonly TextBox _groupTextBox;
    private readonly TextBox _userNameTextBox;
    private readonly TextBox _secretTextBox;
    private readonly TextBox _urlTextBox;
    private readonly TextBox _hostTextBox;
    private readonly TextBox _emailTextBox;
    private readonly TextBox _portTextBox;
    private readonly TextBox _notesTextBox;
    private readonly TextBox _createdTextBox;
    private readonly TextBox _modifiedTextBox;
    private readonly ListView _customFieldsListView;
    private readonly Button _toggleSecretButton;
    private readonly Button _copyUserNameButton;
    private readonly Button _copySecretButton;
    private readonly Button _copyUrlButton;
    private readonly Button _copyHostButton;
    private readonly Button _copyEmailButton;
    private readonly Button _copyCustomFieldButton;
    private readonly Control _urlFieldContainer;
    private readonly Control _hostFieldContainer;
    private readonly Control _emailFieldContainer;
    private readonly Control _portFieldContainer;
    private readonly ClipboardProtectionService _clipboardProtectionService;
    private readonly UiTimer _clipboardClearTimer;

    private EntryDetailViewModel? _currentDetails;
    private bool _showSecret;
    private string? _pendingSensitiveClipboardValue;
    private ClipboardCopyPolicy? _pendingClipboardPolicy;

    /// <summary>
    /// Meldet angeklickte Tags an die Hauptoberfläche zurück, damit diese sofort die Suche anpassen kann.
    /// </summary>
    public event Action<string>? TagClicked;

    /// <summary>
    /// Meldet Status- oder Feedback-Texte an die Hauptoberfläche zurück.
    /// </summary>
    public event Action<string>? StatusMessageRequested;

    /// <summary>
    /// Baut die komplette rechte Detailansicht mit Anzeigen, Copy-Aktionen und Tag-Bereich auf.
    /// </summary>
    public EntryDetailsPanel()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(32, 39, 49);

        _titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 34,
            Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
            ForeColor = Color.WhiteSmoke,
            Text = "Kein Eintrag ausgewählt",
        };

        _typeLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            ForeColor = Color.Silver,
            Text = string.Empty,
        };

        _tagFlowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
            Margin = new Padding(0),
            Padding = new Padding(0),
        };

        _groupTextBox = CreateReadOnlyTextBox();
        _userNameTextBox = CreateReadOnlyTextBox();
        _secretTextBox = CreateReadOnlyTextBox();
        _secretTextBox.PasswordChar = '●';
        _urlTextBox = CreateReadOnlyTextBox();
        _hostTextBox = CreateReadOnlyTextBox();
        _emailTextBox = CreateReadOnlyTextBox();
        _portTextBox = CreateReadOnlyTextBox();
        _notesTextBox = CreateReadOnlyTextBox(multiline: true);
        _createdTextBox = CreateReadOnlyTextBox();
        _modifiedTextBox = CreateReadOnlyTextBox();

        _customFieldsListView = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            HideSelection = false,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };
        _customFieldsListView.Columns.Add("Feld", 180);
        _customFieldsListView.Columns.Add("Wert", 320);
        _customFieldsListView.Columns.Add("Typ", 120);
        _customFieldsListView.DoubleClick += (_, _) => CopySelectedCustomField();

        _clipboardProtectionService = new ClipboardProtectionService();
        _clipboardClearTimer = new UiTimer
        {
            Interval = 1000,
        };
        _clipboardClearTimer.Tick += (_, _) => TryClearClipboard();

        _toggleSecretButton = CreateActionButton("Anzeigen");
        _toggleSecretButton.Click += (_, _) => ToggleSecretVisibility();

        _copyUserNameButton = CreateActionButton("Kopieren");
        _copyUserNameButton.Click += (_, _) => CopyValue(_currentDetails?.UserName, ClipboardCopyKind.UserName);

        _copySecretButton = CreateActionButton("Kopieren");
        // Sensible Copy-Aktionen bleiben bewusst zentral in einer Methode gebündelt,
        // damit spätere Clipboard-Schutzlogik nur an einer Stelle geändert werden muss.
        _copySecretButton.Click += (_, _) => CopyValue(_currentDetails?.SecretValue, ClipboardCopyKind.Secret);

        _copyUrlButton = CreateActionButton("Kopieren");
        _copyUrlButton.Click += (_, _) => CopyValue(_currentDetails?.PrimaryUrl, ClipboardCopyKind.Url);

        _copyHostButton = CreateActionButton("Kopieren");
        _copyHostButton.Click += (_, _) => CopyValue(_currentDetails?.PrimaryHost, ClipboardCopyKind.Host);

        _copyEmailButton = CreateActionButton("Kopieren");
        _copyEmailButton.Click += (_, _) => CopyValue(_currentDetails?.PrimaryEmail, ClipboardCopyKind.Email);

        _copyCustomFieldButton = CreateActionButton("Ausgewählten Wert kopieren");
        _copyCustomFieldButton.Click += (_, _) => CopySelectedCustomField();

        _urlFieldContainer = CreateFieldPanel("URL", _urlTextBox, _copyUrlButton);
        _hostFieldContainer = CreateFieldPanel("Host", _hostTextBox, _copyHostButton);
        _emailFieldContainer = CreateFieldPanel("E-Mail", _emailTextBox, _copyEmailButton);
        _portFieldContainer = CreateFieldPanel("Port", _portTextBox);

        var customFieldsPanel = CreateListPanel("Zusatzfelder", _customFieldsListView, _copyCustomFieldButton);
        var tagsPanel = CreateTagsPanel();

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 14,
            BackColor = BackColor,
            AutoScroll = true,
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(_titleLabel, 0, 0);
        layout.Controls.Add(_typeLabel, 0, 1);
        layout.Controls.Add(tagsPanel, 0, 2);
        layout.Controls.Add(CreateFieldPanel("Gruppe", _groupTextBox), 0, 3);
        layout.Controls.Add(CreateFieldPanel("Benutzername", _userNameTextBox, _copyUserNameButton), 0, 4);
        layout.Controls.Add(CreateFieldPanel("Passwort / Secret", _secretTextBox, _toggleSecretButton, _copySecretButton), 0, 5);
        layout.Controls.Add(_urlFieldContainer, 0, 6);
        layout.Controls.Add(_hostFieldContainer, 0, 7);
        layout.Controls.Add(_emailFieldContainer, 0, 8);
        layout.Controls.Add(_portFieldContainer, 0, 9);
        layout.Controls.Add(customFieldsPanel, 0, 10);
        layout.Controls.Add(CreateFieldPanel("Notizen", _notesTextBox), 0, 11);
        layout.Controls.Add(CreateFieldPanel("Erstellt", _createdTextBox), 0, 12);
        layout.Controls.Add(CreateFieldPanel("Geändert", _modifiedTextBox), 0, 13);

        Controls.Add(layout);
        ClearDetails();
    }

    /// <summary>
    /// Gibt Timer- und Event-Ressourcen wieder frei.
    /// </summary>
    /// <param name="disposing">Gibt an, ob verwaltete Ressourcen freigegeben werden sollen.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clipboardClearTimer.Stop();
            _clipboardClearTimer.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Zeigt die übergebenen Detaildaten im Panel an und blendet optionale Felder passend ein oder aus.
    /// </summary>
    public void DisplayEntry(EntryDetailViewModel details)
    {
        ArgumentNullException.ThrowIfNull(details);

        _currentDetails = details;
        _showSecret = false;

        _titleLabel.Text = details.Title;
        _typeLabel.Text = details.EntryType;
        _groupTextBox.Text = details.GroupPath;
        _userNameTextBox.Text = details.UserName;
        _notesTextBox.Text = details.Notes;
        _createdTextBox.Text = details.CreatedDisplay;
        _modifiedTextBox.Text = details.ModifiedDisplay;
        _urlTextBox.Text = details.PrimaryUrl;
        _hostTextBox.Text = details.PrimaryHost;
        _emailTextBox.Text = details.PrimaryEmail;
        _portTextBox.Text = details.PrimaryPort;

        UpdateOptionalField(_urlFieldContainer, details.PrimaryUrl);
        UpdateOptionalField(_hostFieldContainer, details.PrimaryHost);
        UpdateOptionalField(_emailFieldContainer, details.PrimaryEmail);
        UpdateOptionalField(_portFieldContainer, details.PrimaryPort);

        RefreshTagButtons();
        RefreshSecretText();
        RefreshCustomFieldList();
    }

    /// <summary>
    /// Leert die Detailanzeige, wenn kein Eintrag mehr ausgewählt ist.
    /// </summary>
    public void ClearDetails()
    {
        _currentDetails = null;
        _showSecret = false;

        _titleLabel.Text = "Kein Eintrag ausgewählt";
        _typeLabel.Text = string.Empty;
        _groupTextBox.Text = string.Empty;
        _userNameTextBox.Text = string.Empty;
        _secretTextBox.Text = string.Empty;
        _urlTextBox.Text = string.Empty;
        _hostTextBox.Text = string.Empty;
        _emailTextBox.Text = string.Empty;
        _portTextBox.Text = string.Empty;
        _notesTextBox.Text = string.Empty;
        _createdTextBox.Text = string.Empty;
        _modifiedTextBox.Text = string.Empty;
        _toggleSecretButton.Text = "Anzeigen";
        _customFieldsListView.Items.Clear();
        _tagFlowPanel.Controls.Clear();
        UpdateOptionalField(_urlFieldContainer, string.Empty);
        UpdateOptionalField(_hostFieldContainer, string.Empty);
        UpdateOptionalField(_emailFieldContainer, string.Empty);
        UpdateOptionalField(_portFieldContainer, string.Empty);
    }

    /// <summary>
    /// Schaltet zwischen maskierter und sichtbarer Secret-Anzeige um.
    /// </summary>
    private void ToggleSecretVisibility()
    {
        if (_currentDetails is null)
        {
            return;
        }

        _showSecret = !_showSecret;
        _toggleSecretButton.Text = _showSecret ? "Verbergen" : "Anzeigen";
        DevLog.WriteLine($"Secret-Anzeige umgeschaltet: sichtbar={_showSecret}");
        RefreshSecretText();
        RefreshCustomFieldList();
    }

    /// <summary>
    /// Aktualisiert den sichtbaren Secret-Text abhängig vom aktuellen Anzeigezustand.
    /// </summary>
    private void RefreshSecretText()
    {
        if (_currentDetails is null)
        {
            _secretTextBox.Text = string.Empty;
            return;
        }

        _secretTextBox.PasswordChar = _showSecret ? '\0' : '●';
        _secretTextBox.Text = _showSecret ? _currentDetails.SecretValue : _currentDetails.SecretPreview;
    }

    /// <summary>
    /// Baut die anklickbaren Tag-Buttons für den aktuellen Eintrag neu auf.
    /// </summary>
    private void RefreshTagButtons()
    {
        _tagFlowPanel.SuspendLayout();
        _tagFlowPanel.Controls.Clear();

        if (_currentDetails is not null)
        {
            foreach (var tag in _currentDetails.TagItems)
            {
                var button = new Button
                {
                    AutoSize = true,
                    Text = tag,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(41, 72, 124),
                    ForeColor = Color.WhiteSmoke,
                    Margin = new Padding(0, 0, 6, 6),
                    Padding = new Padding(10, 4, 10, 4),
                    Tag = tag,
                };
                button.Click += (_, _) => RaiseTagClicked(tag);
                _tagFlowPanel.Controls.Add(button);
            }
        }

        _tagFlowPanel.ResumeLayout();
    }

    /// <summary>
    /// Reicht einen ausgewählten Tag an die Hauptoberfläche weiter.
    /// </summary>
    private void RaiseTagClicked(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return;
        }

        DevLog.WriteLine($"Tag aus Details ausgewählt: {tag}");
        TagClicked?.Invoke(tag);
    }

    /// <summary>
    /// Aktualisiert die Liste der Zusatzfelder für den aktuell ausgewählten Eintrag.
    /// </summary>
    private void RefreshCustomFieldList()
    {
        _customFieldsListView.BeginUpdate();
        _customFieldsListView.Items.Clear();

        if (_currentDetails is not null)
        {
            foreach (var field in _currentDetails.CustomFields)
            {
                var displayValue = field.IsSecret && !_showSecret ? field.DisplayValue : field.Value;
                var item = new ListViewItem(field.Name);
                item.SubItems.Add(displayValue);
                item.SubItems.Add(field.Kind);
                item.Tag = field;
                _customFieldsListView.Items.Add(item);
            }
        }

        _customFieldsListView.EndUpdate();
    }

    /// <summary>
    /// Kopiert den aktuell ausgewählten Zusatzfeldwert mit der passenden Schutzrichtlinie.
    /// </summary>
    private void CopySelectedCustomField()
    {
        if (_customFieldsListView.SelectedItems.Count == 0 || _customFieldsListView.SelectedItems[0].Tag is not EntryDetailFieldViewModel field)
        {
            RaiseStatusMessageRequested("Bitte zuerst ein Zusatzfeld auswählen.");
            return;
        }

        var copyKind = field.IsSecret ? ClipboardCopyKind.SecretCustomField : ClipboardCopyKind.CustomField;
        var copiedMessage = field.IsSecret
            ? $"Geheimes Zusatzfeld kopiert: {field.Name}"
            : $"Zusatzfeld kopiert: {field.Name}";

        CopyValue(field.Value, copyKind, copiedMessage);
    }

    /// <summary>
    /// Kopiert einen Wert in die Zwischenablage und wendet die passende Schutzrichtlinie an.
    /// </summary>
    /// <param name="value">Der zu kopierende Wert.</param>
    /// <param name="copyKind">Die fachliche Art der Copy-Aktion.</param>
    /// <param name="copiedMessageOverride">Optionaler Meldungstext für die Benutzeroberfläche.</param>
    private void CopyValue(string? value, ClipboardCopyKind copyKind, string? copiedMessageOverride = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            RaiseStatusMessageRequested("Kein Wert zum Kopieren vorhanden.");
            return;
        }

        var policy = _clipboardProtectionService.GetPolicy(copyKind);

        try
        {
            Clipboard.SetText(value);
            DevLog.WriteLine(copiedMessageOverride ?? policy.CopiedMessage);
            RaiseStatusMessageRequested(copiedMessageOverride ?? policy.CopiedMessage);

            if (policy.ShouldAutoClear)
            {
                ScheduleClipboardClear(value, policy);
            }
            else
            {
                CancelClipboardClear();
            }
        }
        catch (Exception exception)
        {
            DevLog.WriteException("Fehler beim Kopieren in die Zwischenablage", exception);
            RaiseStatusMessageRequested("Kopieren in die Zwischenablage fehlgeschlagen.");
        }
    }

    /// <summary>
    /// Plant das automatische Leeren der Zwischenablage für einen sensiblen Wert.
    /// </summary>
    private void ScheduleClipboardClear(string expectedValue, ClipboardCopyPolicy policy)
    {
        _pendingSensitiveClipboardValue = expectedValue;
        _pendingClipboardPolicy = policy;
        _clipboardClearTimer.Stop();
        _clipboardClearTimer.Interval = Math.Max(1000, (int)policy.AutoClearDelay.TotalMilliseconds);
        _clipboardClearTimer.Start();
    }

    /// <summary>
    /// Bricht einen noch ausstehenden Auto-Clear-Vorgang ab.
    /// </summary>
    private void CancelClipboardClear()
    {
        _clipboardClearTimer.Stop();
        _pendingSensitiveClipboardValue = null;
        _pendingClipboardPolicy = null;
    }

    /// <summary>
    /// Leert die Zwischenablage nur dann, wenn noch derselbe sensible Wert vorhanden ist.
    /// </summary>
    private void TryClearClipboard()
    {
        _clipboardClearTimer.Stop();

        if (string.IsNullOrEmpty(_pendingSensitiveClipboardValue) || _pendingClipboardPolicy is null)
        {
            return;
        }

        try
        {
            if (Clipboard.ContainsText() && string.Equals(Clipboard.GetText(), _pendingSensitiveClipboardValue, StringComparison.Ordinal))
            {
                Clipboard.Clear();
                DevLog.WriteLine(_pendingClipboardPolicy.ClearedMessage);
                RaiseStatusMessageRequested(_pendingClipboardPolicy.ClearedMessage);
            }
        }
        catch (Exception exception)
        {
            DevLog.WriteException("Fehler beim automatischen Leeren der Zwischenablage", exception);
        }
        finally
        {
            _pendingSensitiveClipboardValue = null;
            _pendingClipboardPolicy = null;
        }
    }

    /// <summary>
    /// Meldet eine Statusnachricht an die Hauptoberfläche zurück.
    /// </summary>
    private void RaiseStatusMessageRequested(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        StatusMessageRequested?.Invoke(message);
    }

    /// <summary>
    /// Blendet optionale Detailbereiche abhängig vom vorhandenen Wert ein oder aus.
    /// </summary>
    private static void UpdateOptionalField(Control container, string? value)
    {
        container.Visible = !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Erzeugt ein standardisiertes schreibgeschütztes Textfeld für die Detailansicht.
    /// </summary>
    private static TextBox CreateReadOnlyTextBox(bool multiline = false)
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            Multiline = multiline,
            ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
            Height = multiline ? 120 : 28,
        };
    }

    /// <summary>
    /// Erzeugt einen einheitlich formatierten Aktionsbutton für Copy- und Anzeigeaktionen.
    /// </summary>
    private static Button CreateActionButton(string text)
    {
        return new Button
        {
            AutoSize = true,
            Text = text,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(45, 86, 160),
            ForeColor = Color.WhiteSmoke,
            Margin = new Padding(6, 0, 0, 0),
        };
    }

    /// <summary>
    /// Baut den Tag-Bereich der Detailansicht.
    /// </summary>
    private Control CreateTagsPanel()
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = Color.Transparent,
        };
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            AutoSize = true,
            Text = "Tags",
            ForeColor = Color.Silver,
            Margin = new Padding(0, 0, 0, 4),
        };

        outer.Controls.Add(label, 0, 0);
        outer.Controls.Add(_tagFlowPanel, 0, 1);
        return outer;
    }

    /// <summary>
    /// Baut einen Standardbereich aus Beschriftung, Anzeigefeld und optionalen Aktionsbuttons.
    /// </summary>
    private static Control CreateFieldPanel(string labelText, Control editor, params Control[] actionButtons)
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = Color.Transparent,
        };
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            AutoSize = true,
            Text = labelText,
            ForeColor = Color.Silver,
            Margin = new Padding(0, 0, 0, 4),
        };

        Control editorContainer;
        if (actionButtons.Length == 0)
        {
            editorContainer = editor;
        }
        else
        {
            var inner = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1 + actionButtons.Length,
                RowCount = 1,
                BackColor = Color.Transparent,
            };
            inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (var index = 0; index < actionButtons.Length; index++)
            {
                inner.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }

            inner.Controls.Add(editor, 0, 0);
            for (var index = 0; index < actionButtons.Length; index++)
            {
                inner.Controls.Add(actionButtons[index], index + 1, 0);
            }

            editorContainer = inner;
        }

        outer.Controls.Add(label, 0, 0);
        outer.Controls.Add(editorContainer, 0, 1);
        return outer;
    }

    /// <summary>
    /// Baut einen Listenbereich mit Beschriftung und optionalen Aktionsbuttons.
    /// </summary>
    private static Control CreateListPanel(string labelText, Control listControl, params Control[] actionButtons)
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = Color.Transparent,
        };
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            AutoSize = true,
            Text = labelText,
            ForeColor = Color.Silver,
            Margin = new Padding(0, 0, 0, 4),
        };

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent,
        };
        foreach (var button in actionButtons)
        {
            buttonsPanel.Controls.Add(button);
        }

        outer.Controls.Add(label, 0, 0);
        outer.Controls.Add(listControl, 0, 1);
        outer.Controls.Add(buttonsPanel, 0, 2);
        return outer;
    }
}
