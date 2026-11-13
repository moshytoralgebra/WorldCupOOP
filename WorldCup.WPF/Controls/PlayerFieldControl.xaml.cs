using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WorldCup.DataLayer.Helpers;
using WorldCup.DataLayer.Models;

namespace WorldCup.WPF.Controls;

public partial class PlayerFieldControl : UserControl
{
    private Player? _player;
    private string? _fifaCode;

    public event Action<Player, string>? PlayerSelected;

    public PlayerFieldControl()
    {
        InitializeComponent();
    }

    public void SetPlayer(Player player, string fifaCode, bool isMyTeam)
    {
        _player   = player;
        _fifaCode = fifaCode;

        playerEllipse.Stroke = new SolidColorBrush(isMyTeam
            ? Color.FromRgb(0x15, 0x65, 0xC0)
            : Color.FromRgb(0xB7, 0x1C, 0x1C));

        string imgPath = PlayerImagePaths.GetImagePath(fifaCode, player.ShirtNumber);

        bool imageLoaded = false;
        if (File.Exists(imgPath))
        {
            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource   = new Uri(imgPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze();
                playerEllipse.Fill   = new ImageBrush(bmp) { Stretch = Stretch.UniformToFill };
                lblNumber.Visibility = Visibility.Collapsed;
                imageLoaded = true;
            }
            catch { }
        }

        if (!imageLoaded)
        {
            playerEllipse.Fill   = new SolidColorBrush(Color.FromRgb(0x78, 0x90, 0x9C));
            lblNumber.Text       = player.ShirtNumber.ToString();
            lblNumber.Visibility = Visibility.Visible;
        }

        string lastName = player.Name.Split(' ').LastOrDefault() ?? player.Name;
        lblName.Text = $"#{player.ShirtNumber} {lastName}";
    }

    private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_player == null || _fifaCode == null) return;

        var ease = new CubicEase { EasingMode = EasingMode.EaseInOut };
        var half = new Duration(TimeSpan.FromSeconds(0.15));

        var grow = new DoubleAnimation(1.0, 1.2, half) { EasingFunction = ease };
        grow.Completed += (_, _) =>
        {
            var shrink = new DoubleAnimation(1.2, 1.0, half) { EasingFunction = ease };
            controlScale.BeginAnimation(ScaleTransform.ScaleXProperty, shrink);
            controlScale.BeginAnimation(ScaleTransform.ScaleYProperty, shrink);
        };
        controlScale.BeginAnimation(ScaleTransform.ScaleXProperty, grow);
        controlScale.BeginAnimation(ScaleTransform.ScaleYProperty, grow);

        PlayerSelected?.Invoke(_player, _fifaCode);
        e.Handled = true;
    }
}
