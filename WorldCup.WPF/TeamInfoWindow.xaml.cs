using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WorldCup.DataLayer.Models;
using WorldCup.WPF.Helpers;

namespace WorldCup.WPF;

public partial class TeamInfoWindow : Window
{
    public TeamInfoWindow(Team team, string language)
    {
        InitializeComponent();
        Populate(team, language);
    }

    private void Populate(Team team, string language)
    {
        var s = LocalizationHelper.GetStrings(language);

        Title             = team.Country;
        lblCountry.Text   = team.Country;
        lblFifaCode.Text  = team.FifaCode;

        lblGamesPlayed.Text  = s["games_played"];
        lblWins.Text         = s["wins"];
        lblDraws.Text        = s["draws"];
        lblLosses.Text       = s["losses"];
        lblGoalsScored.Text  = s["goals_scored"];
        lblGoalsConceded.Text= s["goals_conceded"];
        lblGoalDiff.Text     = s["goal_difference"];
        btnClose.Content     = s["close"];

        valGamesPlayed.Text  = team.GamesPlayed.ToString();
        valWins.Text         = team.Wins.ToString();
        valDraws.Text        = team.Draws.ToString();
        valLosses.Text       = team.Losses.ToString();
        valGoalsScored.Text  = team.GoalsFor.ToString();
        valGoalsConceded.Text= team.GoalsAgainst.ToString();
        valGoalDiff.Text     = team.GoalDifferential > 0
                               ? $"+{team.GoalDifferential}"
                               : team.GoalDifferential.ToString();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e) => Close();

    private void TeamInfoWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
        var duration = new Duration(TimeSpan.FromSeconds(0.5));

        var fade = new DoubleAnimation(0.0, 1.0, duration) { EasingFunction = ease };
        BeginAnimation(OpacityProperty, fade);

        var scaleX = new DoubleAnimation(0.85, 1.0, duration) { EasingFunction = ease };
        var scaleY = new DoubleAnimation(0.85, 1.0, duration) { EasingFunction = ease };
        rootScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);
        rootScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);
    }
}
