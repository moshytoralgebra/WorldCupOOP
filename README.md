# World Cup Statistics

A desktop application for exploring FIFA World Cup team, match, and player statistics from the **2018 Men's World Cup** and the **2019 Women's World Cup**. Built with **C# and .NET 8**, the solution includes two separate desktop interfaces: a Windows Forms application and a WPF application, both backed by a shared data layer.

The application supports team and match exploration, player information, favorites, tournament rankings, and PDF export.

## Applications

The solution contains two desktop applications with different ways of exploring the tournament data.

### Windows Forms application

- Browse national teams and their players.
- View starting players and substitutes for a selected team.
- Add up to three players to a favorites panel.
- Move players between favorites and the full player list using drag and drop, selection controls, or the player context menu.
- Select multiple players using Ctrl-click.
- View team-specific rankings for top scorers and yellow cards, including player appearances.
- Compare match attendance across the selected team's matches.
- Export rankings and attendance tables to a formatted PDF.
- Change tournament, language, and favorite team in settings.
- Replace a player's image with a custom local image.

### WPF application

- Select a national team and one of its tournament opponents.
- View the result of their match.
- Display both teams' starting lineups on a visual football pitch, arranged by player position.
- Select a player on the pitch to open a detailed player profile.
- View player information including shirt number, position, captain status, goals, and yellow cards.
- Open team information with games played, wins, draws, losses, goals scored, goals conceded, and goal difference.
- Configure tournament, language, and application window size.
- Use localized English and Croatian interface text.

## Tech Stack

| Category | Technologies |
|---|---|
| Language | C# |
| Framework | .NET 8 |
| Desktop UI | Windows Forms, Windows Presentation Foundation (WPF) |
| Data serialization | System.Text.Json |
| Data access | HttpClient, local JSON files |
| PDF generation | PdfSharp 6.2.4 |
| UI markup | XAML |
| Architecture | Shared data layer, service abstraction, factory pattern |

## Architecture

The solution is organized into three projects:

```text
WorldCupOOP/
│
├── WorldCup.sln
│
├── WorldCup.DataLayer/
│   ├── Config/
│   │   └── dataconfig.txt
│   ├── Data/
│   │   ├── men/
│   │   │   ├── matches.json
│   │   │   └── results.json
│   │   └── women/
│   │       ├── matches.json
│   │       └── results.json
│   ├── Helpers/
│   ├── Models/
│   └── Services/
│       ├── ApiDataService.cs
│       ├── JsonFileDataService.cs
│       ├── DataServiceFactory.cs
│       ├── SettingsService.cs
│       └── ...
│
├── WorldCup.Forms/
│   ├── Controls/
│   ├── Helpers/
│   ├── MainForm.cs
│   └── Program.cs
│
└── WorldCup.WPF/
    ├── Controls/
    ├── Helpers/
    ├── MainWindow.xaml
    ├── PlayerDetailWindow.xaml
    ├── TeamInfoWindow.xaml
    └── App.xaml
```

### Project responsibilities

- **WorldCup.DataLayer** — Shared domain models, JSON data transfer objects, data service interfaces and implementations, configuration, and settings persistence.
- **WorldCup.Forms** — Windows Forms interface for player browsing, favorites, statistical rankings, and PDF export.
- **WorldCup.WPF** — WPF interface for team comparisons, match results, visual lineups, and team/player detail windows.

Both UI projects reference the same data layer. The `IDataService` interface keeps data retrieval separate from presentation, while `DataServiceFactory` selects the implementation based on configuration.

## Data Sources

The application supports two data modes.

### Local JSON files (default)

The repository includes tournament data under:

- `WorldCup.DataLayer/Data/men/`
- `WorldCup.DataLayer/Data/women/`

The default configuration is:

```properties
mode=file
json_base_path=Data/
```

In file mode, team results are loaded from `results.json` and match details from `matches.json`. The data represents the 2018 men's and 2019 women's tournaments and is available locally, so the application does not need an active API connection to browse the included datasets.

### External API mode

The project also includes `ApiDataService`, which retrieves team and match data over HTTP from the configured World Cup API endpoint.

To switch modes, edit:

`WorldCup.DataLayer/Config/dataconfig.txt`

For API mode:

```properties
mode=api
json_base_path=Data/
```

The API service depends on the availability and compatibility of the external endpoint. Local file mode is the reliable default because the required tournament data is included in the repository.

## Getting Started

### Requirements

- Windows
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 with the .NET desktop development workload, or another compatible .NET development environment

The applications target `net8.0-windows` and use Windows-specific desktop UI frameworks.

### 1. Clone the repository

```bash
git clone https://github.com/moshytoralgebra/WorldCupOOP.git
cd WorldCupOOP
```

### 2. Check the data configuration

Open:

`WorldCup.DataLayer/Config/dataconfig.txt`

The default settings use the JSON files included in the repository:

```properties
mode=file
json_base_path=Data/
```

No database installation or external service configuration is required for the default file-based mode.

### 3. Run the Windows Forms application

```bash
dotnet run --project WorldCup.Forms
```

### 4. Run the WPF application

In a separate terminal:

```bash
dotnet run --project WorldCup.WPF
```

You can also open `WorldCup.sln` in Visual Studio, choose either desktop project as the startup project, and run it from the IDE.

## Settings and User Data

Application preferences are stored locally in the user's application data directory:

`%AppData%/WorldCupApp/`

The settings file stores:

- Selected tournament gender (`men` or `women`).
- Interface language (`en` or `hr`).
- Favorite team FIFA code.
- Favorite player names (up to three in the Windows Forms application).

The WPF application also stores the selected window size in `windowsize.txt`.

Player images are loaded from the application's `PlayerImages` directory. The Windows Forms interface allows a user to replace an image with a local JPG, JPEG, PNG, or BMP file; the chosen image is saved as a PNG for that player.

## Statistics and PDF Export

The Windows Forms application calculates player statistics from the selected team's match data:

- Goals scored by each player.
- Yellow cards received by each player.
- Number of match appearances.
- Match attendance, sorted from highest to lowest.

The **Print Rankings** action exports the currently displayed top-scorer, yellow-card, and attendance tables into a multi-page PDF. The PDF includes the selected team, tournament year, export date, and page numbering, with English or Croatian labels depending on the application language.

## Technical Concepts Demonstrated

- Object-oriented programming with C#.
- .NET solution and multi-project structure.
- Separation of concerns through a shared data layer.
- Interfaces and dependency inversion through `IDataService`.
- Factory pattern for selecting data providers.
- Asynchronous programming with `async`/`await` and `HttpClient`.
- JSON deserialization and DTO-to-domain-model mapping.
- LINQ for filtering, grouping, sorting, and aggregating match data.
- Windows Forms controls, custom user controls, and drag-and-drop interactions.
- WPF, XAML, data-driven UI composition, and Canvas-based lineup layout.
- Local application settings and file handling.
- PDF generation with PdfSharp.

## Notes

- The included JSON datasets are historical tournament data, not live scores or current team statistics.
- The optional external API implementation uses an HTTP endpoint. Its availability is outside the application, so local JSON mode is recommended for a reproducible setup.
- The project targets Windows because both user interfaces depend on Windows desktop frameworks.

## Author

**Mariia Oshytor**

[GitHub Profile](https://github.com/moshytoralgebra) · [Repository](https://github.com/moshytoralgebra/WorldCupOOP)
