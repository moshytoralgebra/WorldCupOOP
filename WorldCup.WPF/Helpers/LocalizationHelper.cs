namespace WorldCup.WPF.Helpers;

internal static class LocalizationHelper
{
    public static Dictionary<string, string> GetStrings(string language)
    {
        bool hr = language == "hr";
        return new Dictionary<string, string>
        {
            ["settings_title"] = hr ? "Postavke"           : "Settings",
            ["championship"]   = hr ? "Natjecanje"          : "Championship",
            ["male"]           = hr ? "Muško (2018)"        : "Male (2018)",
            ["female"]         = hr ? "Žensko (2019)"       : "Female (2019)",
            ["language"]       = hr ? "Jezik"               : "Language",
            ["window_size"]    = hr ? "Veličina prozora"    : "Window Size",
            ["fullscreen"]     = hr ? "Cijeli zaslon"       : "Fullscreen",
            ["window_large"]   = hr ? "Veliko (1280x720)"   : "Large (1280x720)",
            ["window_medium"]  = hr ? "Srednje (1024x768)"  : "Medium (1024x768)",
            ["window_small"]   = hr ? "Malo (800x600)"      : "Small (800x600)",
            ["confirm"]        = hr ? "Potvrdi"             : "Confirm",
            ["cancel"]         = hr ? "Odustani"            : "Cancel",
            ["main_title"]     = hr ? "Svjetski Kup"        : "World Cup",
            ["file"]           = hr ? "Datoteka"            : "File",
            ["settings"]       = hr ? "Postavke"            : "Settings",
            ["exit"]           = hr ? "Izlaz"               : "Exit",
            ["your_team"]      = hr ? "Vaša reprezentacija:": "Your Team:",
            ["opponent"]       = hr ? "Protivnik:"          : "Opponent:",
            ["loading"]        = hr ? "Učitavanje..."       : "Loading...",
            ["team_info"]      = hr ? "Informacije o timu"  : "Team Info",
            ["error_loading"]  = hr ? "Greška pri učitavanju" : "Error loading data",
            ["games_played"]   = hr ? "Odigrane utakmice"   : "Games Played",
            ["wins"]           = hr ? "Pobjede"              : "Wins",
            ["draws"]          = hr ? "Neriješeno"           : "Draws",
            ["losses"]         = hr ? "Porazi"               : "Losses",
            ["goals_scored"]   = hr ? "Postignuti golovi"    : "Goals Scored",
            ["goals_conceded"] = hr ? "Primljeni golovi"     : "Goals Conceded",
            ["goal_difference"]= hr ? "Gol razlika"          : "Goal Difference",
            ["close"]          = hr ? "Zatvori"              : "Close",
            ["apply_settings_confirm"] = hr
                ? "Primijeniti nove postavke? Podaci će se osvježiti."
                : "Apply new settings? Data will be reloaded.",
            ["exit_confirm"]  = hr ? "Jeste li sigurni da želite izaći?" : "Are you sure you want to exit?",
            ["exit_title"]    = hr ? "Izlaz"    : "Exit",
            ["apply"]         = hr ? "Primijeni" : "Apply",
            ["number"]         = hr ? "Broj"                 : "Number",
            ["position"]       = hr ? "Pozicija"             : "Position",
            ["captain"]        = hr ? "Kapetan"              : "Captain",
            ["yes"]            = hr ? "Da"                   : "Yes",
            ["no"]             = hr ? "Ne"                   : "No",
            ["goals"]          = hr ? "Golovi"               : "Goals",
            ["yellow_cards"]   = hr ? "Žuti kartoni"         : "Yellow Cards",
            ["pos_goalie"]     = hr ? "Vratar"               : "Goalie",
            ["pos_defender"]   = hr ? "Branič"               : "Defender",
            ["pos_midfield"]   = hr ? "Vezni igrač"          : "Midfielder",
            ["pos_forward"]    = hr ? "Napadač"              : "Forward",
        };
    }
}
