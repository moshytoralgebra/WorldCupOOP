using System.Drawing.Imaging;
using WorldCup.DataLayer.Helpers;
using WorldCup.DataLayer.Models;

namespace WorldCup.Forms.Controls;

public partial class PlayerUserControl : UserControl
{
    private static readonly Color ColFavBackground = Color.FromArgb(0xFF, 0xFD, 0xE7);
    private static readonly Color ColFavBorder     = Color.FromArgb(0xFF, 0xD7, 0x00);
    private static readonly Color ColSelBackground = Color.FromArgb(0xE3, 0xF2, 0xFD);
    private static readonly Color ColSelBorder     = Color.FromArgb(0x42, 0xA5, 0xF5);
    private static readonly Color ColNormalBorder  = Color.LightGray;

    private bool   _isFavorite;
    private bool   _isSelected;
    private bool   _isHovered;
    private string _language = "en";
    private string _fifaCode = string.Empty;

    private Point _dragStartPoint;
    private bool  _isDragging;

    private ToolStripMenuItem _ctxFavItem = null!;

    private static Bitmap? _defaultImage;

    public event EventHandler<CardClickEventArgs>? CardClicked;

    public event EventHandler? FavoriteToggleRequested;

    public event EventHandler<Player>? ImageChanged;

    public Player Player { get; private set; } = new();

    public bool IsFavorite
    {
        get => _isFavorite;
        set
        {
            _isFavorite = value;
            lblStar.Visible = value;
            UpdateContextMenuText();
            UpdateVisualState();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; UpdateVisualState(); }
    }

    public PlayerUserControl()
    {
        InitializeComponent();
        BuildContextMenu();
        ForwardChildMouseEvents();
        pbPlayerImage.DoubleClick += (_, _) => HandleImageDoubleClick();
    }

    public void SetPlayer(Player player, bool isFavorite, string language, string fifaCode)
    {
        Player    = player;
        _language = language;
        _fifaCode = fifaCode;

        if (string.IsNullOrEmpty(player.ImagePath))
        {
            var candidate = $"{fifaCode}_{player.ShirtNumber}.png";
            var candidatePath = PlayerImagesPath(candidate);
            if (File.Exists(candidatePath))
                player.ImagePath = candidate;
        }

        lblName.Text       = $"#{player.ShirtNumber} - {player.Name}";
        lblPosition.Text   = LocalizePosition(player.Position, language);
        lblCaptain.Visible = player.Captain;
        lblCaptain.Text    = language == "hr" ? "© Kapetan" : "© Captain";

        pbPlayerImage.Image = LoadPlayerImage(player.ImagePath);

        IsFavorite = isFavorite;
    }

    private static string PlayerImagesPath(string filename) =>
        Path.Combine(PlayerImagePaths.GetFolder(), filename);

    private static Image LoadPlayerImage(string? imagePath)
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            var full = PlayerImagesPath(imagePath);
            if (File.Exists(full))
            {
                try
                {
                    var bytes = File.ReadAllBytes(full);
                    using var ms  = new MemoryStream(bytes);
                    using var src = Image.FromStream(ms);
                    var bmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(bmp))
                        g.DrawImage(src, 0, 0, src.Width, src.Height);
                    return bmp;
                }
                catch { }
            }
        }

        _defaultImage ??= CreateDefaultPlayerBitmap();
        System.Diagnostics.Debug.Assert(
            _defaultImage != null && _defaultImage.Width > 0 && _defaultImage.Height > 0,
            "Default player image bitmap is invalid.");
        return _defaultImage;
    }

    private static Bitmap CreateDefaultPlayerBitmap()
    {
        var bmp = new Bitmap(120, 120);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.FromArgb(238, 238, 238));
        using var brush = new SolidBrush(Color.FromArgb(189, 189, 189));
        g.FillEllipse(brush, 40, 10, 40, 40);
        g.FillEllipse(brush, 20, 54, 80, 58);
        return bmp;
    }

    protected override void OnDoubleClick(EventArgs e)
    {
        base.OnDoubleClick(e);
        HandleImageDoubleClick();
    }

    private void HandleImageDoubleClick()
    {
        bool hr = _language == "hr";

        using var dlg = new OpenFileDialog
        {
            Title  = hr ? "Odaberite sliku igrača" : "Select player image",
            Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp"
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        try
        {
            var fileName = $"{_fifaCode}_{Player.ShirtNumber}.png";
            var destPath = PlayerImagePaths.GetImagePath(_fifaCode, Player.ShirtNumber);

            var bytes = File.ReadAllBytes(dlg.FileName);
            using var ms  = new MemoryStream(bytes);
            using var src = Image.FromStream(ms);
            src.Save(destPath, ImageFormat.Png);

            Player.ImagePath    = fileName;
            pbPlayerImage.Image = LoadPlayerImage(fileName);

            ImageChanged?.Invoke(this, Player);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not save the image: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string LocalizePosition(string position, string language)
    {
        if (language == "hr")
            return position switch
            {
                "Goalie"   => "Vratar",
                "Defender" => "Branič",
                "Midfield" => "Vezni igrač",
                "Forward"  => "Napadač",
                _          => position
            };

        return position switch
        {
            "Goalie"   => "Goalie",
            "Defender" => "Defender",
            "Midfield" => "Midfielder",
            "Forward"  => "Forward",
            _          => position
        };
    }

    private void BuildContextMenu()
    {
        _ctxFavItem       = new ToolStripMenuItem();
        _ctxFavItem.Click += (_, _) => FavoriteToggleRequested?.Invoke(this, EventArgs.Empty);

        var ctx = new ContextMenuStrip();
        ctx.Items.Add(_ctxFavItem);
        ContextMenuStrip = ctx;

        UpdateContextMenuText();
    }

    private void UpdateContextMenuText()
    {
        if (_ctxFavItem == null) return;
        bool hr = _language == "hr";
        _ctxFavItem.Text = _isFavorite
            ? (hr ? "Ukloni iz favorita" : "Remove from favorites")
            : (hr ? "Dodaj u favourite"  : "Add to favorites");
    }

    private void UpdateVisualState()
    {
        Color baseColor = _isSelected ? ColSelBackground
                        : _isFavorite ? ColFavBackground
                        : Color.White;
        BackColor = _isHovered ? DarkenColor(baseColor) : baseColor;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Color borderColor;
        float borderWidth;

        if (_isSelected)      { borderColor = ColSelBorder; borderWidth = 2f; }
        else if (_isFavorite) { borderColor = ColFavBorder; borderWidth = 2f; }
        else                  { borderColor = _isHovered ? Color.DarkGray : ColNormalBorder; borderWidth = 1f; }

        using var pen = new Pen(borderColor, borderWidth);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }

    private void ForwardChildMouseEvents()
    {
        foreach (Control child in Controls)
        {
            child.MouseEnter += (_, _) => HandleMouseEnter();
            child.MouseLeave += (_, _) => HandleMouseLeave();
            child.MouseClick += (_, _) => { if (!_isDragging) RaiseCardClicked(); };
            child.MouseDown  += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && s is Control c)
                    _dragStartPoint = PointToClient(c.PointToScreen(e.Location));
            };
            child.MouseMove  += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && s is Control c)
                    CheckStartDrag(PointToClient(c.PointToScreen(e.Location)));
            };
        }
    }

    protected override void OnMouseEnter(EventArgs e)  { base.OnMouseEnter(e); HandleMouseEnter(); }
    protected override void OnMouseLeave(EventArgs e)  { base.OnMouseLeave(e); HandleMouseLeave(); }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left) _dragStartPoint = e.Location;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.Button == MouseButtons.Left) CheckStartDrag(e.Location);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        if (!_isDragging) RaiseCardClicked();
    }

    private void CheckStartDrag(Point clientPt)
    {
        if (_isDragging) return;
        if (Math.Abs(clientPt.X - _dragStartPoint.X) > SystemInformation.DragSize.Width  ||
            Math.Abs(clientPt.Y - _dragStartPoint.Y) > SystemInformation.DragSize.Height)
        {
            _isDragging = true;
            DoDragDrop(this, DragDropEffects.Move);
            _isDragging = false;
        }
    }

    private void HandleMouseEnter()
    {
        _isHovered = true;
        UpdateVisualState();
    }

    private void HandleMouseLeave()
    {
        if (!ClientRectangle.Contains(PointToClient(Cursor.Position)))
        {
            _isHovered = false;
            UpdateVisualState();
        }
    }

    private void RaiseCardClicked() =>
        CardClicked?.Invoke(this, new CardClickEventArgs((Control.ModifierKeys & Keys.Control) != 0));

    private static Color DarkenColor(Color color, float factor = 0.06f) =>
        Color.FromArgb(
            color.A,
            Math.Max(0, (int)(color.R * (1 - factor))),
            Math.Max(0, (int)(color.G * (1 - factor))),
            Math.Max(0, (int)(color.B * (1 - factor))));
}
