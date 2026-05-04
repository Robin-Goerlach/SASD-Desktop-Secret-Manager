using Sasd.SecretManager.Application;
using Sasd.SecretManager.Security;

namespace Sasd.SecretManager.WinForms;

/// <summary>
/// Kleiner Lesedialog für einen Eintrag.
/// In diesem frühen Stand dient er nur der ruhigen Anzeige, noch nicht der Bearbeitung.
/// </summary>
public sealed class EntryDetailsDialog : Form
{
    public EntryDetailsDialog(EntryDetailViewModel details)
    {
        ArgumentNullException.ThrowIfNull(details);

        Text = $"Eintrag: {details.Title}";
        Width = 760;
        Height = 640;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(640, 520);
        BackColor = Color.FromArgb(25, 30, 38);
        ForeColor = Color.Gainsboro;

        DevLog.WriteLine($"Detaildialog geöffnet: {details.Title}");

        var propertyGrid = new PropertyGrid
        {
            Dock = DockStyle.Fill,
            ToolbarVisible = false,
            HelpVisible = true,
            BackColor = Color.FromArgb(32, 39, 49),
            ForeColor = Color.Gainsboro,
            ViewBackColor = Color.FromArgb(32, 39, 49),
            ViewForeColor = Color.Gainsboro,
            SelectedObject = details,
        };

        var closeButton = new Button
        {
            Text = "Schließen",
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            DialogResult = DialogResult.OK,
            BackColor = Color.FromArgb(44, 93, 145),
            ForeColor = Color.WhiteSmoke,
            FlatStyle = FlatStyle.Flat,
            Padding = new Padding(12, 6, 12, 6),
        };

        closeButton.FlatAppearance.BorderSize = 0;

        var footerPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12),
            BackColor = Color.FromArgb(18, 22, 29),
        };

        footerPanel.Controls.Add(closeButton);

        Controls.Add(propertyGrid);
        Controls.Add(footerPanel);
        AcceptButton = closeButton;
    }
}
