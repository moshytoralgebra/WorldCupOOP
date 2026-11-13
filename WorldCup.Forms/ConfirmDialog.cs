namespace WorldCup.Forms;

public sealed class ConfirmDialog : Form
{
    public ConfirmDialog(string message, string title, string confirmText, string cancelText)
    {
        var lblMsg = new Label
        {
            Text      = message,
            AutoSize  = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Location  = new Point(20, 18),
            Size      = new Size(320, 72)
        };

        var btnConfirm = new Button
        {
            Text         = confirmText,
            DialogResult = DialogResult.OK,
            Location     = new Point(178, 107),
            Size         = new Size(80, 28),
            UseVisualStyleBackColor = true
        };

        var btnCancel = new Button
        {
            Text         = cancelText,
            DialogResult = DialogResult.Cancel,
            Location     = new Point(268, 107),
            Size         = new Size(80, 28),
            UseVisualStyleBackColor = true
        };

        AcceptButton      = btnConfirm;
        CancelButton      = btnCancel;
        Text              = title;
        ClientSize        = new Size(368, 150);
        FormBorderStyle   = FormBorderStyle.FixedDialog;
        MaximizeBox       = false;
        MinimizeBox       = false;
        StartPosition     = FormStartPosition.CenterParent;
        AutoScaleMode     = AutoScaleMode.Font;
        AutoScaleDimensions = new SizeF(7F, 15F);

        Controls.Add(lblMsg);
        Controls.Add(btnConfirm);
        Controls.Add(btnCancel);
    }
}
