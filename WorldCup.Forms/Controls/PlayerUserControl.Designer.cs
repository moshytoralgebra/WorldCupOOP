namespace WorldCup.Forms.Controls;

partial class PlayerUserControl
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pbPlayerImage = new PictureBox();
        lblStar = new Label();
        lblName = new Label();
        lblPosition = new Label();
        lblCaptain = new Label();

        ((System.ComponentModel.ISupportInitialize)pbPlayerImage).BeginInit();
        SuspendLayout();

        pbPlayerImage.Location = new Point(30, 8);
        pbPlayerImage.Name = "pbPlayerImage";
        pbPlayerImage.Size = new Size(120, 120);
        pbPlayerImage.SizeMode = PictureBoxSizeMode.Zoom;
        pbPlayerImage.TabStop = false;
        pbPlayerImage.BackColor = Color.FromArgb(238, 238, 238);

        lblStar.AutoSize = false;
        lblStar.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
        lblStar.ForeColor = Color.FromArgb(0xFF, 0xD7, 0x00);
        lblStar.Location = new Point(152, 3);
        lblStar.Name = "lblStar";
        lblStar.Size = new Size(24, 24);
        lblStar.Text = "★";
        lblStar.TextAlign = ContentAlignment.MiddleCenter;
        lblStar.Visible = false;

        lblName.AutoSize = false;
        lblName.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblName.Location = new Point(5, 133);
        lblName.Name = "lblName";
        lblName.Size = new Size(170, 38);
        lblName.Text = string.Empty;
        lblName.TextAlign = ContentAlignment.TopCenter;

        lblPosition.AutoSize = false;
        lblPosition.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
        lblPosition.ForeColor = Color.Gray;
        lblPosition.Location = new Point(5, 175);
        lblPosition.Name = "lblPosition";
        lblPosition.Size = new Size(170, 20);
        lblPosition.Text = string.Empty;
        lblPosition.TextAlign = ContentAlignment.MiddleCenter;

        lblCaptain.AutoSize = false;
        lblCaptain.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
        lblCaptain.ForeColor = Color.DarkBlue;
        lblCaptain.Location = new Point(5, 197);
        lblCaptain.Name = "lblCaptain";
        lblCaptain.Size = new Size(170, 18);
        lblCaptain.Text = "© Captain";
        lblCaptain.TextAlign = ContentAlignment.MiddleCenter;
        lblCaptain.Visible = false;

        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        Controls.Add(pbPlayerImage);
        Controls.Add(lblStar);
        Controls.Add(lblName);
        Controls.Add(lblPosition);
        Controls.Add(lblCaptain);
        Cursor = Cursors.Hand;
        Name = "PlayerUserControl";
        Size = new Size(180, 220);

        ((System.ComponentModel.ISupportInitialize)pbPlayerImage).EndInit();
        ResumeLayout(false);
    }

    private PictureBox pbPlayerImage;
    private Label lblStar;
    private Label lblName;
    private Label lblPosition;
    private Label lblCaptain;
}
