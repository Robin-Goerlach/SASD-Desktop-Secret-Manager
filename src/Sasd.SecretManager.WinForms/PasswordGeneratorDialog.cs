using Sasd.SecretManager.Application;

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// WinForms-Dialog für den Passwortgenerator.
/// </summary>
/// <remarks>
/// Der Dialog enthält bewusst nur UI-Logik: Eingaben lesen, Optionen anzeigen,
/// Ergebnis übernehmen oder kopieren. Die eigentliche Generierung liegt in
/// <see cref="PasswordGeneratorService"/> in der Application-Schicht.
/// Dadurch bleibt die Sicherheitslogik testbar und von Windows Forms getrennt.
/// </remarks>
public sealed class PasswordGeneratorDialog : Form
{
    private readonly PasswordGeneratorService _generatorService = new();

    private readonly NumericUpDown _lengthNumericUpDown;
    private readonly CheckBox _uppercaseCheckBox;
    private readonly CheckBox _lowercaseCheckBox;
    private readonly CheckBox _digitsCheckBox;
    private readonly CheckBox _symbolsCheckBox;
    private readonly CheckBox _excludeAmbiguousCheckBox;
    private readonly CheckBox _requireEveryGroupCheckBox;
    private readonly TextBox _symbolsTextBox;
    private readonly TextBox _passwordTextBox;
    private readonly Label _statusLabel;

    /// <summary>
    /// Enthält nach dem Bestätigen das zuletzt erzeugte Passwort.
    /// </summary>
    public GeneratedPassword? GeneratedPassword { get; private set; }

    /// <summary>
    /// Initialisiert den Generator-Dialog mit sicheren Voreinstellungen.
    /// </summary>
    public PasswordGeneratorDialog()
    {
        Text = "Passwortgenerator";
        Width = 760;
        Height = 520;
        MinimumSize = new Size(680, 460);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;

        _lengthNumericUpDown = new NumericUpDown
        {
            Minimum = PasswordGenerationOptions.MinimumLength,
            Maximum = PasswordGenerationOptions.MaximumLength,
            Value = PasswordGenerationOptions.DefaultLength,
            Width = 90,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };

        _uppercaseCheckBox = CreateCheckBox("Großbuchstaben", isChecked: true);
        _lowercaseCheckBox = CreateCheckBox("Kleinbuchstaben", isChecked: true);
        _digitsCheckBox = CreateCheckBox("Ziffern", isChecked: true);
        _symbolsCheckBox = CreateCheckBox("Sonderzeichen", isChecked: true);
        _excludeAmbiguousCheckBox = CreateCheckBox("Verwechselbare Zeichen vermeiden", isChecked: true);
        _requireEveryGroupCheckBox = CreateCheckBox("Mindestens ein Zeichen aus jeder gewählten Gruppe", isChecked: true);

        _symbolsTextBox = CreateTextBox("!#$%&()*+,-./:;<=>?@[]^_{|}~");
        _passwordTextBox = CreateTextBox(string.Empty);
        _passwordTextBox.ReadOnly = true;
        _passwordTextBox.Font = new Font(FontFamily.GenericMonospace, 11f, FontStyle.Regular);

        _statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            ForeColor = Color.Silver,
            Text = "Bereit. Das Passwort wird lokal und ohne Netzwerkzugriff erzeugt.",
            TextAlign = ContentAlignment.MiddleLeft,
        };

        var generateButton = CreateButton("Neu generieren");
        generateButton.Click += (_, _) => GenerateAndShowPassword();

        var copyButton = CreateButton("Kopieren");
        copyButton.Click += (_, _) => CopyPasswordToClipboard();

        var acceptButton = CreateButton("Übernehmen");
        acceptButton.Click += (_, _) => AcceptGeneratedPassword();

        var closeButton = CreateButton("Schließen");
        closeButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(BuildRootLayout(generateButton, copyButton, acceptButton, closeButton));

        AcceptButton = acceptButton;
        CancelButton = closeButton;

        // Direkt beim Öffnen ein Passwort erzeugen, damit der Benutzer nicht
        // erst eine zusätzliche Aktion ausführen muss. Die Optionen bleiben
        // trotzdem jederzeit anpassbar.
        GenerateAndShowPassword();
    }

    private Control BuildRootLayout(Button generateButton, Button copyButton, Button acceptButton, Button closeButton)
    {
        var descriptionLabel = new Label
        {
            Dock = DockStyle.Top,
            AutoSize = false,
            Height = 56,
            ForeColor = Color.Silver,
            Text = "Erzeugt ein kryptographisch zufälliges Passwort. " +
                   "Die Generierung erfolgt in der Application-Schicht mit RandomNumberGenerator.",
        };

        var formLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 8,
            Padding = new Padding(16),
            BackColor = BackColor,
        };

        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        formLayout.Controls.Add(CreateLabel("Länge"), 0, 0);
        formLayout.Controls.Add(_lengthNumericUpDown, 1, 0);
        formLayout.Controls.Add(CreateLabel("Zeichengruppen"), 0, 1);
        formLayout.Controls.Add(BuildCharacterGroupPanel(), 1, 1);
        formLayout.Controls.Add(CreateLabel("Sicherheitsoptionen"), 0, 2);
        formLayout.Controls.Add(BuildSecurityOptionsPanel(), 1, 2);
        formLayout.Controls.Add(CreateLabel("Erlaubte Sonderzeichen"), 0, 3);
        formLayout.Controls.Add(_symbolsTextBox, 1, 3);
        formLayout.Controls.Add(CreateLabel("Ergebnis"), 0, 4);
        formLayout.Controls.Add(_passwordTextBox, 1, 4);
        formLayout.Controls.Add(CreateLabel("Hinweis"), 0, 5);
        formLayout.Controls.Add(_statusLabel, 1, 5);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 58,
            Padding = new Padding(16, 10, 16, 10),
            BackColor = Color.Transparent,
        };

        buttonPanel.Controls.Add(closeButton);
        buttonPanel.Controls.Add(acceptButton);
        buttonPanel.Controls.Add(copyButton);
        buttonPanel.Controls.Add(generateButton);

        var rootPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = BackColor,
        };

        rootPanel.Controls.Add(formLayout);
        rootPanel.Controls.Add(descriptionLabel);
        rootPanel.Controls.Add(buttonPanel);

        return rootPanel;
    }

    private Control BuildCharacterGroupPanel()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Color.Transparent,
        };

        panel.Controls.Add(_uppercaseCheckBox);
        panel.Controls.Add(_lowercaseCheckBox);
        panel.Controls.Add(_digitsCheckBox);
        panel.Controls.Add(_symbolsCheckBox);

        return panel;
    }

    private Control BuildSecurityOptionsPanel()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
        };

        panel.Controls.Add(_excludeAmbiguousCheckBox);
        panel.Controls.Add(_requireEveryGroupCheckBox);

        return panel;
    }

    private void GenerateAndShowPassword()
    {
        try
        {
            var options = CreateOptionsFromForm();
            GeneratedPassword = _generatorService.Generate(options);
            _passwordTextBox.Text = GeneratedPassword.Value;
            _statusLabel.Text = $"Passwort erzeugt: {GeneratedPassword.Length} Zeichen, " +
                                $"{GeneratedPassword.SelectedCharacterGroupCount} Zeichengruppen.";
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            GeneratedPassword = null;
            _passwordTextBox.Clear();
            _statusLabel.Text = exception.Message;
            MessageBox.Show(
                this,
                exception.Message,
                "Passwortgenerator",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private PasswordGenerationOptions CreateOptionsFromForm()
    {
        return new PasswordGenerationOptions
        {
            Length = (int)_lengthNumericUpDown.Value,
            IncludeUppercase = _uppercaseCheckBox.Checked,
            IncludeLowercase = _lowercaseCheckBox.Checked,
            IncludeDigits = _digitsCheckBox.Checked,
            IncludeSymbols = _symbolsCheckBox.Checked,
            ExcludeAmbiguousCharacters = _excludeAmbiguousCheckBox.Checked,
            RequireEverySelectedCharacterGroup = _requireEveryGroupCheckBox.Checked,
            CustomSymbols = _symbolsTextBox.Text,
        };
    }

    private void CopyPasswordToClipboard()
    {
        if (string.IsNullOrEmpty(_passwordTextBox.Text))
        {
            return;
        }

        Clipboard.SetText(_passwordTextBox.Text);
        _statusLabel.Text = "Passwort in die Zwischenablage kopiert. Auto-Clear erfolgt hier noch nicht global.";
    }

    private void AcceptGeneratedPassword()
    {
        if (string.IsNullOrEmpty(_passwordTextBox.Text))
        {
            GenerateAndShowPassword();
        }

        if (string.IsNullOrEmpty(_passwordTextBox.Text))
        {
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private static Label CreateLabel(string text)
    {
        return new Label
        {
            AutoSize = true,
            Text = text,
            ForeColor = Color.Gainsboro,
            Margin = new Padding(0, 8, 12, 8),
        };
    }

    private static TextBox CreateTextBox(string value)
    {
        return new TextBox
        {
            Text = value,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };
    }

    private static CheckBox CreateCheckBox(string text, bool isChecked)
    {
        return new CheckBox
        {
            AutoSize = true,
            Checked = isChecked,
            Text = text,
            ForeColor = Color.Gainsboro,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 2, 18, 2),
        };
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
