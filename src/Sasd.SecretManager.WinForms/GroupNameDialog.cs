// ============================================================================
// Dateiüberblick:
// Kleiner Hilfsdialog für Gruppennamen.
// Diese Kommentarfassung ergänzt den bestehenden Quellcode um zusätzliche
// Orientierungshinweise, ohne die fachliche Logik zu verändern.
// ============================================================================

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Kleiner Dialog zur Erfassung eines Gruppennamens.
/// </summary>
public sealed class GroupNameDialog : Form
{
    private readonly TextBox _nameTextBox;

    /// <summary>
    /// Initialisiert den Gruppennamen-Dialog mit Titel, Beschreibung und Vorbelegung.
    /// </summary>
    public GroupNameDialog(string title, string description, string initialName)
    {
        Text = title;
        Width = 560;
        Height = 250;
        MinimumSize = new Size(480, 220);
        StartPosition = FormStartPosition.CenterParent;

        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;

        _nameTextBox = new TextBox
        {
            Dock = DockStyle.Top,
            Text = initialName,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(24, 28, 36),
            ForeColor = Color.Gainsboro,
            Margin = new Padding(0, 0, 0, 12),
        };

        var descriptionLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 54,
            Text = description,
            ForeColor = Color.Silver,
        };

        var nameLabel = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Text = "Gruppenname",
            ForeColor = Color.Gainsboro,
            Margin = new Padding(0, 8, 0, 6),
        };

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 54,
            Padding = new Padding(0, 10, 0, 0),
            BackColor = Color.Transparent,
        };

        var okButton = CreateButton("Speichern");
        okButton.Click += (_, _) => SaveAndClose();
        var cancelButton = CreateButton("Abbrechen");
        cancelButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        buttonPanel.Controls.Add(okButton);
        buttonPanel.Controls.Add(cancelButton);

        var contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            BackColor = BackColor,
        };
        contentPanel.Controls.Add(_nameTextBox);
        contentPanel.Controls.Add(nameLabel);
        contentPanel.Controls.Add(descriptionLabel);

        Controls.Add(contentPanel);
        Controls.Add(buttonPanel);

        AcceptButton = okButton;
        CancelButton = cancelButton;
    }

    public string GroupName { get; private set; } = string.Empty;

    private void SaveAndClose()
    {
        if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
        {
            MessageBox.Show(this, "Bitte einen Gruppennamen angeben.", "SASD Secret Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _nameTextBox.Focus();
            return;
        }

        GroupName = _nameTextBox.Text.Trim();
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
            Margin = new Padding(8, 0, 0, 0),
            Padding = new Padding(12, 6, 12, 6),
        };
    }
}
