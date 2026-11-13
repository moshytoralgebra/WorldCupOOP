using PdfSharp.Fonts;

namespace WorldCup.Forms.Helpers;

public class SystemFontResolver : IFontResolver
{
    public static readonly SystemFontResolver Instance = new();

    public string DefaultFaceName => "Arial";

    public byte[] GetFont(string faceName)
    {
        string fileName = faceName switch
        {
            "Arial"        => "arial.ttf",
            "Arial-Bold"   => "arialbd.ttf",
            "Arial-Italic" => "ariali.ttf",
            _              => "arial.ttf"
        };
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
            fileName);
        return File.ReadAllBytes(path);
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
        string face = (bold, italic) switch
        {
            (true, _) => "Arial-Bold",
            (_, true) => "Arial-Italic",
            _          => "Arial"
        };
        return new FontResolverInfo(face);
    }
}
