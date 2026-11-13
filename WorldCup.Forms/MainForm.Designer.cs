namespace WorldCup.Forms;

partial class MainForm
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
        menuStripMain    = new MenuStrip();
        menuFile         = new ToolStripMenuItem();
        menuSettings     = new ToolStripMenuItem();
        var menuSep      = new ToolStripSeparator();
        menuExit         = new ToolStripMenuItem();

        pnlTop           = new Panel();
        lblSelectTeam    = new Label();
        cbTeams          = new ComboBox();
        pbLoading        = new ProgressBar();
        lblError         = new Label();

        tabMain          = new TabControl();
        tabPagePlayers   = new TabPage();
        tabPageRankings  = new TabPage();

        splitContainer   = new SplitContainer();
        lblFavTitle      = new Label();
        flpFavorites     = new FlowLayoutPanel();
        lblAllTitle      = new Label();
        flpAllPlayers    = new FlowLayoutPanel();
        pnlButtons       = new Panel();
        btnMoveFavs      = new Button();
        btnMoveAll       = new Button();

        pnlPrintBar      = new Panel();
        btnPrint         = new Button();
        splitRankings    = new SplitContainer();
        splitPlayerStats = new SplitContainer();
        lblTopScorers    = new Label();
        lvTopScorers     = new ListView();
        lblYellowCards   = new Label();
        lvYellowCards    = new ListView();
        lblAttendance    = new Label();
        lvAttendance     = new ListView();

        menuStripMain.SuspendLayout();
        pnlTop.SuspendLayout();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        splitRankings.Panel1.SuspendLayout();
        splitRankings.Panel2.SuspendLayout();
        splitRankings.SuspendLayout();
        splitPlayerStats.Panel1.SuspendLayout();
        splitPlayerStats.Panel2.SuspendLayout();
        splitPlayerStats.SuspendLayout();
        pnlButtons.SuspendLayout();
        pnlPrintBar.SuspendLayout();
        tabPagePlayers.SuspendLayout();
        tabPageRankings.SuspendLayout();
        tabMain.SuspendLayout();
        SuspendLayout();

        menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuSettings, menuSep, menuExit });
        menuFile.Name = "menuFile"; menuFile.Text = "File";

        menuSettings.Name = "menuSettings"; menuSettings.Text = "Settings";
        menuSettings.Click += menuSettings_Click;

        menuExit.Name = "menuExit"; menuExit.Text = "Exit";
        menuExit.Click += menuExit_Click;

        menuStripMain.Items.Add(menuFile);
        menuStripMain.Location = new Point(0, 0);
        menuStripMain.Name = "menuStripMain";
        menuStripMain.Size = new Size(1024, 24);
        menuStripMain.TabIndex = 0;

        lblSelectTeam.AutoSize = true;
        lblSelectTeam.Location = new Point(10, 17);
        lblSelectTeam.Name = "lblSelectTeam"; lblSelectTeam.TabIndex = 0;
        lblSelectTeam.Text = "Select your favorite team:";

        cbTeams.DropDownStyle  = ComboBoxStyle.DropDownList;
        cbTeams.FormattingEnabled = true;
        cbTeams.Location = new Point(240, 14);
        cbTeams.Name = "cbTeams"; cbTeams.Size = new Size(300, 23); cbTeams.TabIndex = 1;
        cbTeams.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        cbTeams.SelectedIndexChanged += cbTeams_SelectedIndexChanged;

        pbLoading.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        pbLoading.Location = new Point(550, 16);
        pbLoading.Name = "pbLoading"; pbLoading.Size = new Size(130, 22);
        pbLoading.Style = ProgressBarStyle.Marquee; pbLoading.MarqueeAnimationSpeed = 30;
        pbLoading.TabIndex = 2; pbLoading.Visible = false;

        lblError.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
        lblError.ForeColor = Color.Red; lblError.Location = new Point(10, 52);
        lblError.Name = "lblError"; lblError.Size = new Size(1004, 22);
        lblError.TabIndex = 3; lblError.Text = string.Empty; lblError.Visible = false;

        pnlTop.Controls.Add(lblSelectTeam);
        pnlTop.Controls.Add(cbTeams);
        pnlTop.Controls.Add(pbLoading);
        pnlTop.Controls.Add(lblError);
        pnlTop.Dock = DockStyle.Top; pnlTop.Height = 80;
        pnlTop.Name = "pnlTop"; pnlTop.TabIndex = 1;
        pnlTop.Padding = new Padding(0, 0, 0, 4);

        lblFavTitle.Dock = DockStyle.Top;
        lblFavTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblFavTitle.Height = 28; lblFavTitle.Name = "lblFavTitle";
        lblFavTitle.Text = "Favorite Players (0/3)";
        lblFavTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblFavTitle.BackColor = Color.FromArgb(0xFF, 0xFD, 0xF0);

        flpFavorites.AllowDrop = true; flpFavorites.AutoScroll = true;
        flpFavorites.BackColor = Color.FromArgb(0xFF, 0xFD, 0xF0);
        flpFavorites.Dock = DockStyle.Fill; flpFavorites.FlowDirection = FlowDirection.LeftToRight;
        flpFavorites.Name = "flpFavorites"; flpFavorites.Padding = new Padding(8);
        flpFavorites.WrapContents = true;
        flpFavorites.DragEnter += Flp_DragEnter;
        flpFavorites.DragDrop  += FlpFavorites_DragDrop;

        splitContainer.Panel1.Controls.Add(flpFavorites);
        splitContainer.Panel1.Controls.Add(lblFavTitle);

        lblAllTitle.Dock = DockStyle.Top;
        lblAllTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblAllTitle.Height = 28; lblAllTitle.Name = "lblAllTitle";
        lblAllTitle.Text = "All Players"; lblAllTitle.TextAlign = ContentAlignment.MiddleCenter;

        flpAllPlayers.AllowDrop = true; flpAllPlayers.AutoScroll = true;
        flpAllPlayers.BackColor = Color.White;
        flpAllPlayers.Dock = DockStyle.Fill; flpAllPlayers.FlowDirection = FlowDirection.LeftToRight;
        flpAllPlayers.Name = "flpAllPlayers"; flpAllPlayers.Padding = new Padding(8);
        flpAllPlayers.WrapContents = true;
        flpAllPlayers.DragEnter += Flp_DragEnter;
        flpAllPlayers.DragDrop  += FlpAllPlayers_DragDrop;

        splitContainer.Panel2.Controls.Add(flpAllPlayers);
        splitContainer.Panel2.Controls.Add(lblAllTitle);

        splitContainer.Dock = DockStyle.Fill; splitContainer.Name = "splitContainer";
        splitContainer.Orientation = Orientation.Vertical;
        splitContainer.SplitterDistance = 500; splitContainer.SplitterWidth = 6;
        splitContainer.TabIndex = 0;

        btnMoveFavs.Location = new Point(10, 8); btnMoveFavs.Name = "btnMoveFavs";
        btnMoveFavs.Size = new Size(260, 28); btnMoveFavs.TabIndex = 0;
        btnMoveFavs.Text = "Move selected to favorites";
        btnMoveFavs.UseVisualStyleBackColor = true;
        btnMoveFavs.Click += btnMoveFavs_Click;

        btnMoveAll.Location = new Point(280, 8); btnMoveAll.Name = "btnMoveAll";
        btnMoveAll.Size = new Size(270, 28); btnMoveAll.TabIndex = 1;
        btnMoveAll.Text = "Move selected to all players";
        btnMoveAll.UseVisualStyleBackColor = true;
        btnMoveAll.Click += btnMoveAll_Click;

        pnlButtons.Controls.Add(btnMoveFavs);
        pnlButtons.Controls.Add(btnMoveAll);
        pnlButtons.Dock = DockStyle.Bottom; pnlButtons.Height = 44;
        pnlButtons.Name = "pnlButtons"; pnlButtons.TabIndex = 1;

        tabPagePlayers.Controls.Add(splitContainer);
        tabPagePlayers.Controls.Add(pnlButtons);
        tabPagePlayers.Name = "tabPagePlayers"; tabPagePlayers.Text = "Players";
        tabPagePlayers.Padding = new Padding(3);

        btnPrint.Location = new Point(10, 8); btnPrint.Name = "btnPrint";
        btnPrint.Size = new Size(160, 28); btnPrint.TabIndex = 0;
        btnPrint.Text = "Print Rankings";
        btnPrint.UseVisualStyleBackColor = true;
        btnPrint.Click += btnPrint_Click;

        pnlPrintBar.Controls.Add(btnPrint);
        pnlPrintBar.Dock = DockStyle.Top; pnlPrintBar.Height = 44;
        pnlPrintBar.Name = "pnlPrintBar"; pnlPrintBar.TabIndex = 0;

        lblTopScorers.Dock = DockStyle.Top;
        lblTopScorers.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblTopScorers.Height = 26; lblTopScorers.Name = "lblTopScorers";
        lblTopScorers.Text = "Top Scorers"; lblTopScorers.TextAlign = ContentAlignment.MiddleCenter;
        lblTopScorers.BackColor = SystemColors.ControlLight;

        lvTopScorers.Dock = DockStyle.Fill; lvTopScorers.FullRowSelect = true;
        lvTopScorers.GridLines = true; lvTopScorers.LabelEdit = false;
        lvTopScorers.MultiSelect = false; lvTopScorers.Name = "lvTopScorers";
        lvTopScorers.View = View.Details; lvTopScorers.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        lvTopScorers.Columns.AddRange(new ColumnHeader[]
        {
            new() { Text = "#",           Width = 36  },
            new() { Text = "Player",      Width = 160 },
            new() { Text = "Goals",       Width = 60  },
            new() { Text = "Appearances", Width = 90  }
        });

        splitPlayerStats.Panel1.Controls.Add(lvTopScorers);
        splitPlayerStats.Panel1.Controls.Add(lblTopScorers);

        lblYellowCards.Dock = DockStyle.Top;
        lblYellowCards.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblYellowCards.Height = 26; lblYellowCards.Name = "lblYellowCards";
        lblYellowCards.Text = "Yellow Cards"; lblYellowCards.TextAlign = ContentAlignment.MiddleCenter;
        lblYellowCards.BackColor = SystemColors.ControlLight;

        lvYellowCards.Dock = DockStyle.Fill; lvYellowCards.FullRowSelect = true;
        lvYellowCards.GridLines = true; lvYellowCards.LabelEdit = false;
        lvYellowCards.MultiSelect = false; lvYellowCards.Name = "lvYellowCards";
        lvYellowCards.View = View.Details; lvYellowCards.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        lvYellowCards.Columns.AddRange(new ColumnHeader[]
        {
            new() { Text = "#",             Width = 36  },
            new() { Text = "Player",        Width = 160 },
            new() { Text = "Yellow Cards",  Width = 90  },
            new() { Text = "Appearances",   Width = 90  }
        });

        splitPlayerStats.Panel2.Controls.Add(lvYellowCards);
        splitPlayerStats.Panel2.Controls.Add(lblYellowCards);

        splitPlayerStats.Dock = DockStyle.Fill; splitPlayerStats.Name = "splitPlayerStats";
        splitPlayerStats.Orientation = Orientation.Vertical;
        splitPlayerStats.SplitterDistance = 380; splitPlayerStats.SplitterWidth = 6;
        splitPlayerStats.TabIndex = 0;

        splitRankings.Panel1.Controls.Add(splitPlayerStats);

        lblAttendance.Dock = DockStyle.Top;
        lblAttendance.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblAttendance.Height = 26; lblAttendance.Name = "lblAttendance";
        lblAttendance.Text = "Attendance"; lblAttendance.TextAlign = ContentAlignment.MiddleCenter;
        lblAttendance.BackColor = SystemColors.ControlLight;

        lvAttendance.Dock = DockStyle.Fill; lvAttendance.FullRowSelect = true;
        lvAttendance.GridLines = true; lvAttendance.LabelEdit = false;
        lvAttendance.MultiSelect = false; lvAttendance.Name = "lvAttendance";
        lvAttendance.View = View.Details; lvAttendance.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        lvAttendance.Columns.AddRange(new ColumnHeader[]
        {
            new() { Text = "#",         Width = 36  },
            new() { Text = "Location",  Width = 200 },
            new() { Text = "Attendance",Width = 100 },
            new() { Text = "Home Team", Width = 150 },
            new() { Text = "Away Team", Width = 150 }
        });

        splitRankings.Panel2.Controls.Add(lvAttendance);
        splitRankings.Panel2.Controls.Add(lblAttendance);

        splitRankings.Dock = DockStyle.Fill; splitRankings.Name = "splitRankings";
        splitRankings.Orientation = Orientation.Horizontal;
        splitRankings.SplitterDistance = 280; splitRankings.SplitterWidth = 6;
        splitRankings.TabIndex = 1;

        tabPageRankings.Controls.Add(splitRankings);
        tabPageRankings.Controls.Add(pnlPrintBar);
        tabPageRankings.Name = "tabPageRankings"; tabPageRankings.Text = "Rankings";
        tabPageRankings.Padding = new Padding(3);

        tabMain.Controls.Add(tabPagePlayers);
        tabMain.Controls.Add(tabPageRankings);
        tabMain.Dock = DockStyle.Fill; tabMain.Name = "tabMain"; tabMain.TabIndex = 2;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode       = AutoScaleMode.Font;
        ClientSize          = new Size(1024, 680);
        MinimumSize         = new Size(900, 600);
        Name = "MainForm"; StartPosition = FormStartPosition.CenterScreen;
        Text = "World Cup";
        Controls.Add(tabMain);
        Controls.Add(pnlTop);
        Controls.Add(menuStripMain);
        MainMenuStrip = menuStripMain;
        ActiveControl = null;
        Load += MainForm_Load;

        menuStripMain.ResumeLayout(false);
        menuStripMain.PerformLayout();
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.ResumeLayout(false);
        splitPlayerStats.Panel1.ResumeLayout(false);
        splitPlayerStats.Panel2.ResumeLayout(false);
        splitPlayerStats.ResumeLayout(false);
        splitRankings.Panel1.ResumeLayout(false);
        splitRankings.Panel2.ResumeLayout(false);
        splitRankings.ResumeLayout(false);
        pnlButtons.ResumeLayout(false);
        pnlPrintBar.ResumeLayout(false);
        tabPagePlayers.ResumeLayout(false);
        tabPageRankings.ResumeLayout(false);
        tabMain.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private MenuStrip         menuStripMain;
    private ToolStripMenuItem menuFile;
    private ToolStripMenuItem menuSettings;
    private ToolStripMenuItem menuExit;
    private Panel             pnlTop;
    private Label             lblSelectTeam;
    private ComboBox          cbTeams;
    private ProgressBar       pbLoading;
    private Label             lblError;
    private TabControl        tabMain;
    private TabPage           tabPagePlayers;
    private TabPage           tabPageRankings;
    private SplitContainer    splitContainer;
    private Label             lblFavTitle;
    private FlowLayoutPanel   flpFavorites;
    private Label             lblAllTitle;
    private FlowLayoutPanel   flpAllPlayers;
    private Panel             pnlButtons;
    private Button            btnMoveFavs;
    private Button            btnMoveAll;
    private Panel             pnlPrintBar;
    private Button            btnPrint;
    private SplitContainer    splitRankings;
    private SplitContainer    splitPlayerStats;
    private Label             lblTopScorers;
    private ListView          lvTopScorers;
    private Label             lblYellowCards;
    private ListView          lvYellowCards;
    private Label             lblAttendance;
    private ListView          lvAttendance;
}
