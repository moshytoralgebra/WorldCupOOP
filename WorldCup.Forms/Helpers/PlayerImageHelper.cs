using System.Drawing.Imaging;
using WorldCup.DataLayer.Helpers;

namespace WorldCup.Forms.Helpers;

internal static class PlayerImageHelper
{
    private static Bitmap? _cachedDefault32;

    public static Image LoadOrDefault(string fifaCode, int shirtNumber, Size size)
    {
        var fullPath = PlayerImagePaths.GetImagePath(fifaCode, shirtNumber);

        if (File.Exists(fullPath))
        {
            try
            {
                var bytes = File.ReadAllBytes(fullPath);
                using var ms  = new MemoryStream(bytes);
                using var src = Image.FromStream(ms);
                var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp))
                    g.DrawImage(src, 0, 0, size.Width, size.Height);
                return bmp;
            }
            catch { }
        }

        return GetDefaultBitmap(size);
    }

    private static Bitmap GetDefaultBitmap(Size size)
    {
        if (size.Width == 32 && size.Height == 32 && _cachedDefault32 != null)
            return _cachedDefault32;

        var bmp = new Bitmap(size.Width, size.Height);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.FromArgb(238, 238, 238));
        using var brush = new SolidBrush(Color.FromArgb(189, 189, 189));
        float w = size.Width, h = size.Height;
        g.FillEllipse(brush, w * 0.33f, h * 0.08f, w * 0.33f, h * 0.33f);
        g.FillEllipse(brush, w * 0.17f, h * 0.45f, w * 0.67f, h * 0.48f);

        if (size.Width == 32 && size.Height == 32)
            _cachedDefault32 = bmp;

        return bmp;
    }
}
