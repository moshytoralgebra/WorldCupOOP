using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WorldCup.DataLayer.Helpers;
using WorldCup.DataLayer.Models;
using WorldCup.WPF.Helpers;

namespace WorldCup.WPF;

public partial class PlayerDetailWindow : Window
{
    public PlayerDetailWindow(Player player, string fifaCode,
                              int goals, int yellowCards,
                              string language, bool isMyTeam)
    {
        InitializeComponent();
        Populate(player, fifaCode, goals, yellowCards, language, isMyTeam);
    }

    private void Populate(Player player, string fifaCode,
                          int goals, int yellowCards,
                          string language, bool isMyTeam)
    {
        var s = LocalizationHelper.GetStrings(language);

        headerGrid.Background = new SolidColorBrush(isMyTeam
            ? Color.FromRgb(0x1B, 0x5E, 0x20)
            : Color.FromRgb(0x7F, 0x00, 0x00));

        lblPlayerName.Text   = player.Name;
        lblPlayerNumber.Text = $"#{player.ShirtNumber}";
        Title                = player.Name;

        string imgPath    = PlayerImagePaths.GetImagePath(fifaCode, player.ShirtNumber);
        bool   imgLoaded  = false;
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
                playerEllipse.Fill        = new ImageBrush(bmp) { Stretch = Stretch.UniformToFill };
                lblPhotoNumber.Visibility = Visibility.Collapsed;
                imgLoaded = true;
            }
            catch { }
        }

        if (!imgLoaded)
        {
            playerEllipse.Fill        = new SolidColorBrush(Color.FromRgb(0x78, 0x90, 0x9C));
            lblPhotoNumber.Text       = player.ShirtNumber.ToString();
            lblPhotoNumber.Visibility = Visibility.Visible;
        }

        lblNameKey.Text     = "Name";
        lblNumberKey.Text   = s["number"];
        lblPositionKey.Text = s["position"];
        lblCaptainKey.Text  = s["captain"];
        lblGoalsKey.Text    = s["goals"];
        lblYellowKey.Text   = s["yellow_cards"];
        btnClose.Content    = s["close"];

        valName.Text     = player.Name;
        valNumber.Text   = $"#{player.ShirtNumber}";
        valPosition.Text = LocalizePosition(player.Position, s);
        valCaptain.Text  = player.Captain ? s["yes"] : s["no"];
        valGoals.Text    = goals.ToString();
        valYellow.Text   = yellowCards.ToString();
    }

    private static string LocalizePosition(string position, Dictionary<string, string> s) =>
        position switch
        {
            "Goalie"   => s["pos_goalie"],
            "Defender" => s["pos_defender"],
            "Midfield" => s["pos_midfield"],
            "Forward"  => s["pos_forward"],
            _          => position
        };

    private void btnClose_Click(object sender, RoutedEventArgs e) => Close();

    private void PlayerDetailWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var ease     = new QuadraticEase { EasingMode = EasingMode.EaseOut };
        var duration = new Duration(TimeSpan.FromSeconds(0.3));

        BeginAnimation(OpacityProperty,
            new DoubleAnimation(0.0, 1.0, duration) { EasingFunction = ease });

        rootTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty,
            new DoubleAnimation(60, 0, duration) { EasingFunction = ease });
    }
}
