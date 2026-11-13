namespace WorldCup.Forms;

partial class SettingsForm
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
        lblGender = new Label();
        rbMale = new RadioButton();
        rbFemale = new RadioButton();
        lblLanguage = new Label();
        cbLanguage = new ComboBox();
        btnConfirm = new Button();
        btnCancel = new Button();
        SuspendLayout();

        lblGender.AutoSize = true;
        lblGender.Location = new Point(20, 18);
        lblGender.Name = "lblGender";
        lblGender.TabIndex = 0;
        lblGender.Text = "Championship";

        rbMale.AutoSize = true;
        rbMale.Location = new Point(20, 42);
        rbMale.Name = "rbMale";
        rbMale.TabIndex = 1;
        rbMale.Text = "Male (2018)";
        rbMale.UseVisualStyleBackColor = true;
        rbMale.CheckedChanged += rbGender_CheckedChanged;

        rbFemale.AutoSize = true;
        rbFemale.Location = new Point(20, 68);
        rbFemale.Name = "rbFemale";
        rbFemale.TabIndex = 2;
        rbFemale.Text = "Female (2019)";
        rbFemale.UseVisualStyleBackColor = true;
        rbFemale.CheckedChanged += rbGender_CheckedChanged;

        lblLanguage.AutoSize = true;
        lblLanguage.Location = new Point(20, 108);
        lblLanguage.Name = "lblLanguage";
        lblLanguage.TabIndex = 3;
        lblLanguage.Text = "Language";

        cbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
        cbLanguage.FormattingEnabled = true;
        cbLanguage.Items.AddRange(new object[] { "English", "Hrvatski" });
        cbLanguage.Location = new Point(20, 130);
        cbLanguage.Name = "cbLanguage";
        cbLanguage.Size = new Size(200, 23);
        cbLanguage.TabIndex = 4;
        cbLanguage.SelectedIndexChanged += cbLanguage_SelectedIndexChanged;

        btnConfirm.Enabled = false;
        btnConfirm.Location = new Point(158, 176);
        btnConfirm.Name = "btnConfirm";
        btnConfirm.Size = new Size(82, 28);
        btnConfirm.TabIndex = 5;
        btnConfirm.Text = "Confirm";
        btnConfirm.UseVisualStyleBackColor = true;
        btnConfirm.Click += btnConfirm_Click;

        btnCancel.Location = new Point(250, 176);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(82, 28);
        btnCancel.TabIndex = 6;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;

        AcceptButton = btnConfirm;
        CancelButton = btnCancel;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(348, 218);
        Controls.Add(lblGender);
        Controls.Add(rbMale);
        Controls.Add(rbFemale);
        Controls.Add(lblLanguage);
        Controls.Add(cbLanguage);
        Controls.Add(btnConfirm);
        Controls.Add(btnCancel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Settings";
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblGender;
    private RadioButton rbMale;
    private RadioButton rbFemale;
    private Label lblLanguage;
    private ComboBox cbLanguage;
    private Button btnConfirm;
    private Button btnCancel;
}
