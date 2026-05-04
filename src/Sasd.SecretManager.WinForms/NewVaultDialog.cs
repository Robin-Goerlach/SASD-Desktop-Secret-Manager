namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Dialog zum Anlegen eines neuen leeren Tresors.
/// </summary>
public sealed class NewVaultDialog : Form
{
    private readonly TextBox _nameTextBox;
    private readonly TextBox _passwordTextBox;
    private readonly TextBox _confirmPasswordTextBox;
    private readonly CheckBox _showPasswordCheckBox;

    public string VaultName => _nameTextBox.Text.Trim();
    public string MasterPassword => _passwordTextBox.Text;

    public NewVaultDialog()
    {
        Text = "Neuen Tresor anlegen";
        Width = 620;
        Height = 340;
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

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
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
