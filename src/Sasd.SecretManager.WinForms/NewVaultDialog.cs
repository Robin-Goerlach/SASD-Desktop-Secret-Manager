using Sasd.SecretManager.Security;

// ============================================================================
// Dateiüberblick:
// Dialog zum Anlegen eines neuen Tresors.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Dialog zum Anlegen eines neuen leeren Tresors.
/// Milestone 5 ergänzt eine direkte Qualitätswarnung
/// für das Master-Passwort.
/// </summary>
public sealed class NewVaultDialog : Form
{
    private readonly TextBox _nameTextBox;
    private readonly TextBox _passwordTextBox;
    private readonly TextBox _confirmPasswordTextBox;
    private readonly CheckBox _showPasswordCheckBox;
    private readonly Label _strengthLabel;

    /// <summary>
    /// Liefert den vom Benutzer erfassten Tresornamen.
    /// </summary>
    public string VaultName => _nameTextBox.Text.Trim();
    public string MasterPassword => _passwordTextBox.Text;

    /// <summary>
    /// Baut den Dialog zum Anlegen eines neuen Tresors mit Namens- und Passwortabfrage auf.
    /// </summary>
    public NewVaultDialog()
    {
        Text = "Neuen Tresor anlegen";
        Width = 620;
        Height = 390;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;

        _nameTextBox = CreateTextBox("Neuer Tresor");
        _passwordTextBox = CreatePasswordTextBox();
        _confirmPasswordTextBox = CreatePasswordTextBox();
        _showPasswordCheckBox = new CheckBox
        {
            AutoSize = true,
            Text = "Passwörter anzeigen",
            ForeColor = Color.Gainsboro,
            BackColor = Color.Transparent,
        };
        _showPasswordCheckBox.CheckedChanged += (_, _) =>
        {
            var passwordChar = _showPasswordCheckBox.Checked ? '\0' : '●';
            _passwordTextBox.PasswordChar = passwordChar;
            _confirmPasswordTextBox.PasswordChar = passwordChar;
        };

        _strengthLabel = new Label
        {
            AutoSize = true,
            ForeColor = Color.Silver,
            BackColor = Color.Transparent,
            MaximumSize = new Size(360, 0),
            Text = "Bitte ein möglichst starkes Master-Passwort wählen.",
        };

        _passwordTextBox.TextChanged += (_, _) => UpdateStrengthFeedback();

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(16),
            BackColor = BackColor,
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        table.Controls.Add(CreateLabel("Tresorname"), 0, 0);
        table.Controls.Add(_nameTextBox, 1, 0);
        table.Controls.Add(CreateLabel("Master-Passwort"), 0, 1);
        table.Controls.Add(_passwordTextBox, 1, 1);
        table.Controls.Add(CreateLabel("Passwort bestätigen"), 0, 2);
        table.Controls.Add(_confirmPasswordTextBox, 1, 2);
        table.Controls.Add(_showPasswordCheckBox, 1, 3);
        table.Controls.Add(_strengthLabel, 1, 4);

        var okButton = CreateButton("Tresor anlegen");
        okButton.Click += (_, _) => ConfirmAndClose();
        var cancelButton = CreateButton("Abbrechen");
        cancelButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 54,
            BackColor = Color.Transparent,
        };
        buttons.Controls.Add(okButton);
        buttons.Controls.Add(cancelButton);

        Controls.Add(table);
        Controls.Add(buttons);
        AcceptButton = okButton;
        CancelButton = cancelButton;
        UpdateStrengthFeedback();
    }

    private void ConfirmAndClose()
    {
        if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
        {
            MessageBox.Show(this, "Bitte einen Tresornamen angeben.", "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _nameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(_passwordTextBox.Text))
        {
            MessageBox.Show(this, "Bitte ein Master-Passwort vergeben.", "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _passwordTextBox.Focus();
            return;
        }

        if (!string.Equals(_passwordTextBox.Text, _confirmPasswordTextBox.Text, StringComparison.Ordinal))
        {
            MessageBox.Show(this, "Passwort und Bestätigung stimmen nicht überein.", "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _confirmPasswordTextBox.Focus();
            return;
        }

        var assessment = PasswordStrengthEvaluator.Assess(_passwordTextBox.Text);
        if (assessment.ShouldWarnBeforeUse)
        {
            var confirmation = MessageBox.Show(
                this,
                $"Das gewählte Master-Passwort wird aktuell als \"{assessment.Summary}\" eingestuft.{Environment.NewLine}{assessment.Recommendation}{Environment.NewLine}{Environment.NewLine}Trotzdem fortfahren?",
                "Schwaches Master-Passwort",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmation != DialogResult.Yes)
            {
                _passwordTextBox.Focus();
                return;
            }
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void UpdateStrengthFeedback()
    {
        var assessment = PasswordStrengthEvaluator.Assess(_passwordTextBox.Text);
        _strengthLabel.Text = $"Einschätzung: {assessment.Summary} · {assessment.Recommendation}";
        _strengthLabel.ForeColor = assessment.Level switch
        {
            PasswordStrengthLevel.VeryWeak => Color.IndianRed,
            PasswordStrengthLevel.Weak => Color.Salmon,
            PasswordStrengthLevel.Moderate => Color.Khaki,
            PasswordStrengthLevel.Good => Color.LightGreen,
            PasswordStrengthLevel.Strong => Color.MediumSpringGreen,
            _ => Color.Silver,
        };
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

    private static TextBox CreateTextBox(string initialValue)
    {
        return new TextBox
        {
            Text = initialValue,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };
    }

    private static TextBox CreatePasswordTextBox()
    {
        var textBox = CreateTextBox(string.Empty);
        textBox.PasswordChar = '●';
        return textBox;
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
            Margin = new Padding(8, 8, 0, 0),
            Padding = new Padding(12, 6, 12, 6),
        };
    }
}
