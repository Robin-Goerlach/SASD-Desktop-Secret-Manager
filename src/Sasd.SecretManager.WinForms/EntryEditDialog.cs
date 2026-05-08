using Sasd.SecretManager.Application;
using Sasd.SecretManager.Domain;
using Sasd.SecretManager.Security;

// ============================================================================
// Dateiüberblick:
// Dialog zum Erstellen und Bearbeiten von Einträgen.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Einfacher Bearbeitungsdialog für neue oder bestehende Einträge.
/// Milestone 7 ergänzt bekannte Tags als Vorschlagsliste.
/// </summary>
public sealed class EntryEditDialog : Form
{
    private readonly EntryValidationService _validationService = new();

    private readonly TextBox _titleTextBox;
    private readonly ComboBox _typeComboBox;
    private readonly TextBox _userNameTextBox;
    private readonly TextBox _secretTextBox;
    private readonly ComboBox _groupComboBox;
    private readonly TextBox _tagsTextBox;
    private readonly ListBox _knownTagsListBox;
    private readonly TextBox _notesTextBox;
    private readonly TextBox _customFieldsTextBox;
    private readonly CheckBox _showSecretCheckBox;

    /// <summary>
    /// Enthält nach erfolgreichem Bestätigen die vom Benutzer erfassten Daten.
    /// </summary>
    public EntryEditModel ResultModel { get; private set; } = new();

    public EntryEditDialog(string title, EntryEditModel model, IReadOnlyList<string> availableGroups, IReadOnlyList<string>? knownTags = null)
    {
        Text = title;
        Width = 920;
        Height = 780;
        MinimumSize = new Size(820, 700);
        StartPosition = FormStartPosition.CenterParent;

        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;

        _titleTextBox = CreateTextBox(model.Title);
        _typeComboBox = CreateComboBox(Enum.GetValues<EntryType>().Cast<object>().ToArray());
        _typeComboBox.SelectedItem = model.EntryType;

        _userNameTextBox = CreateTextBox(model.UserName);
        _secretTextBox = CreateTextBox(model.Secret);
        _secretTextBox.PasswordChar = '●';
        _secretTextBox.Width = 360;

        _groupComboBox = CreateComboBox(availableGroups.Cast<object>().ToArray());
        if (!string.IsNullOrWhiteSpace(model.SelectedGroupPath))
        {
            _groupComboBox.SelectedItem = model.SelectedGroupPath;
        }

        _tagsTextBox = CreateTextBox(model.TagsText);
        _knownTagsListBox = new ListBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
            BorderStyle = BorderStyle.FixedSingle,
            Height = 110,
        };
        _knownTagsListBox.Items.AddRange((knownTags ?? Array.Empty<string>())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .Cast<object>()
            .ToArray());
        _knownTagsListBox.DoubleClick += (_, _) => AddSelectedKnownTag();

        _notesTextBox = CreateTextBox(model.Notes, multiline: true);
        _customFieldsTextBox = CreateTextBox(model.CustomFieldsText, multiline: true);
        _showSecretCheckBox = new CheckBox
        {
            AutoSize = true,
            Text = "Secret anzeigen",
            ForeColor = Color.Gainsboro,
            BackColor = Color.Transparent,
        };
        _showSecretCheckBox.CheckedChanged += (_, _) => _secretTextBox.PasswordChar = _showSecretCheckBox.Checked ? '\0' : '●';

        var customFieldHint = new Label
        {
            AutoSize = true,
            ForeColor = Color.Silver,
            Text = "Zusatzfelder: eine Zeile pro Feld, z. B. Host = db.example.org oder !API Secret = abc123",
            Margin = new Padding(0, 2, 0, 10),
        };

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 54,
            Padding = new Padding(0, 10, 0, 0),
            BackColor = Color.Transparent,
        };

        var saveButton = CreateButton("Speichern");
        saveButton.Click += (_, _) => SaveAndClose();
        var cancelButton = CreateButton("Abbrechen");
        cancelButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.Controls.Add(cancelButton);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 8,
            Padding = new Padding(16),
            BackColor = BackColor,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(CreateLabel("Titel"), 0, 0);
        layout.Controls.Add(_titleTextBox, 1, 0);
        layout.Controls.Add(CreateLabel("Typ"), 0, 1);
        layout.Controls.Add(_typeComboBox, 1, 1);
        layout.Controls.Add(CreateLabel("Benutzername"), 0, 2);
        layout.Controls.Add(_userNameTextBox, 1, 2);
        layout.Controls.Add(CreateLabel("Passwort / Secret"), 0, 3);
        layout.Controls.Add(CreateSecretPanel(), 1, 3);
        layout.Controls.Add(CreateLabel("Gruppe"), 0, 4);
        layout.Controls.Add(_groupComboBox, 1, 4);
        layout.Controls.Add(CreateLabel("Tags"), 0, 5);
        layout.Controls.Add(CreateTagsPanel(), 1, 5);
        layout.Controls.Add(CreateLabel("Notizen"), 0, 6);
        layout.Controls.Add(_notesTextBox, 1, 6);
        layout.Controls.Add(CreateLabel("Zusatzfelder"), 0, 7);
        layout.Controls.Add(CreateCustomFieldsPanel(customFieldHint), 1, 7);

        var rootPanel = new Panel { Dock = DockStyle.Fill, BackColor = BackColor };
        rootPanel.Controls.Add(layout);
        rootPanel.Controls.Add(buttonsPanel);

        Controls.Add(rootPanel);
        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private Control CreateSecretPanel()
    {
        // DSM-001:
        // Neben dem Secret-Feld wird ein Generator-Button angezeigt.
        // Der Generator schreibt nur in diesen Dialog; erst Speichern uebernimmt
        // den Wert wirklich in das EntryEditModel.
        var generateButton = CreateButton("Generieren");
        generateButton.AutoSize = true;
        generateButton.Click += (_, _) => ShowPasswordGeneratorForEntry();

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = Color.Transparent,
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        panel.Controls.Add(_secretTextBox, 0, 0);
        panel.Controls.Add(generateButton, 1, 0);
        panel.Controls.Add(_showSecretCheckBox, 2, 0);

        return panel;
    }

    private void ShowPasswordGeneratorForEntry()
    {
        // DSM-001:
        // Der Generator veraendert den Eintrag nicht sofort. Er setzt nur den Text
        // im Bearbeitungsdialog. Erst wenn der Benutzer den Dialog mit "Speichern"
        // bestaetigt, wird das neue Secret in das EntryEditModel uebernommen.
        using var dialog = new PasswordGeneratorDialog();

        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.GeneratedPassword is null)
        {
            return;
        }

        _secretTextBox.Text = dialog.GeneratedPassword.Value;
        DevLog.Info("Generiertes Passwort übernommen.");
        _secretTextBox.SelectionStart = _secretTextBox.TextLength;
        _secretTextBox.Focus();
    }

    private Control CreateTagsPanel()
    {
        var addButton = CreateButton("Tag übernehmen");
        addButton.AutoSize = true;
        addButton.Click += (_, _) => AddSelectedKnownTag();

        var knownTagsLabel = new Label
        {
            AutoSize = true,
            ForeColor = Color.Silver,
            Text = "Bekannte Tags des Tresors",
            Margin = new Padding(0, 8, 0, 4),
        };

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Color.Transparent,
        };
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(_tagsTextBox, 0, 0);
        panel.Controls.Add(knownTagsLabel, 0, 1);
        panel.Controls.Add(_knownTagsListBox, 0, 2);
        panel.Controls.Add(addButton, 0, 3);
        return panel;
    }

    private void AddSelectedKnownTag()
    {
        if (_knownTagsListBox.SelectedItem is not string selectedTag || string.IsNullOrWhiteSpace(selectedTag))
        {
            return;
        }

        var existingTags = _tagsTextBox.Text
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!existingTags.Contains(selectedTag, StringComparer.OrdinalIgnoreCase))
        {
            existingTags.Add(selectedTag);
        }

        existingTags.Sort(StringComparer.OrdinalIgnoreCase);
        _tagsTextBox.Text = string.Join(", ", existingTags);
        _tagsTextBox.SelectionStart = _tagsTextBox.TextLength;
        _tagsTextBox.Focus();
    }

    private Control CreateCustomFieldsPanel(Control hintLabel)
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent,
        };
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(hintLabel, 0, 0);
        panel.Controls.Add(_customFieldsTextBox, 0, 1);
        return panel;
    }

    private void SaveAndClose()
    {
        // DSM-003:
        // Der Dialog prüft alle rein syntaktischen Eingaberegeln, bevor er sich
        // schließt. Dadurch verliert der Benutzer seine Eingabe nicht, wenn z. B.
        // ein Zusatzfeld falsch geschrieben wurde. Tresorabhängige Regeln wie
        // doppelte Titel prüft danach zusätzlich der Application-Service.
        var candidateModel = BuildResultModelFromControls();
        var validationResult = _validationService.ValidateStandalone(candidateModel);
        if (!validationResult.IsValid)
        {
            ShowValidationWarning(validationResult);
            FocusFirstInvalidField(validationResult);
            return;
        }

        ResultModel = candidateModel;
        DialogResult = DialogResult.OK;
        Close();
    }

    private EntryEditModel BuildResultModelFromControls()
    {
        return new EntryEditModel
        {
            Title = _titleTextBox.Text.Trim(),
            EntryType = _typeComboBox.SelectedItem is EntryType entryType ? entryType : EntryType.Login,
            UserName = _userNameTextBox.Text.Trim(),
            Secret = _secretTextBox.Text,
            SelectedGroupPath = _groupComboBox.SelectedItem?.ToString() ?? string.Empty,
            TagsText = _tagsTextBox.Text,
            Notes = _notesTextBox.Text,
            CustomFieldsText = _customFieldsTextBox.Text,
        };
    }

    private void ShowValidationWarning(EntryValidationResult validationResult)
    {
        var message = "Der Eintrag kann noch nicht gespeichert werden:"
            + Environment.NewLine
            + Environment.NewLine
            + string.Join(
                Environment.NewLine,
                validationResult.Errors.Select(issue => "• " + issue.Message));

        DevLog.Info("Eintragsdialog: Validierung fehlgeschlagen.");
        MessageBox.Show(
            this,
            message,
            "SASD Secret Manager",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    private void FocusFirstInvalidField(EntryValidationResult validationResult)
    {
        var propertyName = validationResult.Errors.FirstOrDefault()?.PropertyName;

        // C# kann bei einem switch-Ausdruck mit verschiedenen konkreten Control-Typen
        // (TextBox, ComboBox, ...) ohne Zieltyp keinen gemeinsamen besten Typ bestimmen.
        // Deshalb geben wir den Zieltyp hier ausdrücklich als Control vor.
        Control targetControl = propertyName switch
        {
            nameof(EntryEditModel.Title) => _titleTextBox,
            nameof(EntryEditModel.UserName) => _userNameTextBox,
            nameof(EntryEditModel.Secret) => _secretTextBox,
            nameof(EntryEditModel.SelectedGroupPath) => _groupComboBox,
            nameof(EntryEditModel.TagsText) => _tagsTextBox,
            nameof(EntryEditModel.CustomFieldsText) => _customFieldsTextBox,
            _ => _titleTextBox,
        };

        targetControl.Focus();
    }

    private static Label CreateLabel(string text)
    {
        return new Label
        {
            AutoSize = true,
            Text = text,
            ForeColor = Color.Gainsboro,
            Margin = new Padding(0, 8, 0, 8),
        };
    }

    private static TextBox CreateTextBox(string value, bool multiline = false)
    {
        return new TextBox
        {
            Text = value,
            Multiline = multiline,
            Height = multiline ? 160 : 28,
            Dock = DockStyle.Fill,
            ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };
    }

    private static ComboBox CreateComboBox(object[] values)
    {
        var comboBox = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };
        comboBox.Items.AddRange(values);
        if (comboBox.Items.Count > 0)
        {
            comboBox.SelectedIndex = 0;
        }

        return comboBox;
    }

    private static Button CreateButton(string text)
    {
        return new Button
        {
            AutoSize = true,
            Text = text,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(45, 86, 160),
            ForeColor = Color.WhiteSmoke,
            Margin = new Padding(8, 0, 0, 0),
            Padding = new Padding(12, 6, 12, 6),
        };
    }
}
