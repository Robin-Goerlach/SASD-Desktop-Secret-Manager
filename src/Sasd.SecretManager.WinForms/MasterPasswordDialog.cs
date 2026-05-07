using Sasd.SecretManager.Security;

// ============================================================================
// Dateiüberblick:
// Abfrage- und Warn-Dialog für Master-Passwörter.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Fragt ein Master-Passwort für Öffnen oder Speichern eines Tresors ab.
/// In Milestone 5 ergänzt der Dialog eine einfache Qualitätswarnung,
/// damit triviale Passwörter nicht stillschweigend bestätigt werden.
/// </summary>
public sealed class MasterPasswordDialog : Form
{
    private readonly TextBox _passwordTextBox;
    private readonly CheckBox _showPasswordCheckBox;
    private readonly Label _strengthLabel;
    private readonly bool _warnOnWeakPassword;

    public string Password => _passwordTextBox.Text;

    /// <summary>
    /// Zeigt einen Dialog zur Eingabe oder Bestätigung eines Master-Passworts.
    /// </summary>
    public MasterPasswordDialog(string title, string description, bool warnOnWeakPassword = false)
    {
        Text = title;
        Width = 560;
        Height = 290;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;
        _warnOnWeakPassword = warnOnWeakPassword;

        var descriptionLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 60,
            ForeColor = Color.Gainsboro,
            Text = description,
            Padding = new Padding(0, 0, 0, 8),
        };

        _passwordTextBox = new TextBox
        {
            Dock = DockStyle.Top,
            PasswordChar = '●',
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
        };
        _passwordTextBox.TextChanged += (_, _) => UpdateStrengthFeedback();

        _showPasswordCheckBox = new CheckBox
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Text = "Passwort anzeigen",
            ForeColor = Color.Gainsboro,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 8, 0, 0),
        };
        _showPasswordCheckBox.CheckedChanged += (_, _) => _passwordTextBox.PasswordChar = _showPasswordCheckBox.Checked ? '\0' : '●';

        _strengthLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 48,
            ForeColor = Color.Silver,
            Padding = new Padding(0, 8, 0, 0),
            Text = warnOnWeakPassword
                ? "Bitte ein möglichst starkes Master-Passwort wählen."
                : string.Empty,
            Visible = warnOnWeakPassword,
        };

        var okButton = CreateButton("OK");
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
            Height = 50,
            BackColor = Color.Transparent,
        };
        buttons.Controls.Add(okButton);
        buttons.Controls.Add(cancelButton);

        var content = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            BackColor = BackColor,
        };
        content.Controls.Add(_strengthLabel);
        content.Controls.Add(_showPasswordCheckBox);
        content.Controls.Add(_passwordTextBox);
        content.Controls.Add(descriptionLabel);

        Controls.Add(content);
        Controls.Add(buttons);

        AcceptButton = okButton;
        CancelButton = cancelButton;
        UpdateStrengthFeedback();
    }

    private void ConfirmAndClose()
    {
        if (string.IsNullOrWhiteSpace(_passwordTextBox.Text))
        {
            MessageBox.Show(this, "Bitte ein Master-Passwort eingeben.", "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _passwordTextBox.Focus();
            return;
        }

        if (_warnOnWeakPassword)
        {
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
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void UpdateStrengthFeedback()
    {
        if (!_warnOnWeakPassword)
        {
            return;
        }

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
