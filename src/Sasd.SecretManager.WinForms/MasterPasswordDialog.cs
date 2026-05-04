namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Fragt ein Master-Passwort für Öffnen oder Speichern eines Tresors ab.
/// </summary>
public sealed class MasterPasswordDialog : Form
{
    private readonly TextBox _passwordTextBox;
    private readonly CheckBox _showPasswordCheckBox;

    public string Password => _passwordTextBox.Text;

    public MasterPasswordDialog(string title, string description)
    {
        Text = title;
        Width = 560;
        Height = 250;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;

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
        content.Controls.Add(_showPasswordCheckBox);
        content.Controls.Add(_passwordTextBox);
        content.Controls.Add(descriptionLabel);

        Controls.Add(content);
        Controls.Add(buttons);

        AcceptButton = okButton;
        CancelButton = cancelButton;
    }

    private void ConfirmAndClose()
    {
        if (string.IsNullOrWhiteSpace(_passwordTextBox.Text))
        {
            MessageBox.Show(this, "Bitte ein Master-Passwort eingeben.", "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _passwordTextBox.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
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
