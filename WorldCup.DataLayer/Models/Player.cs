namespace WorldCup.DataLayer.Models;

public class Player
{
    public string Name { get; set; } = string.Empty;
    public bool Captain { get; set; }
    public int ShirtNumber { get; set; }
    public string Position { get; set; } = string.Empty;
    // Stores just the filename, e.g. "ENG_10.png".
    // Full path at runtime: Path.Combine(BaseDirectory, "PlayerImages", ImagePath)
    public string? ImagePath { get; set; }
}
