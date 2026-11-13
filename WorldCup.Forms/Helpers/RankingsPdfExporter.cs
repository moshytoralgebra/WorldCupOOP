using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;

namespace WorldCup.Forms.Helpers;

internal static class RankingsPdfExporter
{
    public static void Export(
        string           filePath,
        string           language,
        string           gender,
        string           teamCountry,
        List<string[]>   topScorers,
        List<string[]>   yellowCards,
        List<string[]>   attendance)
    {
        if (GlobalFontSettings.FontResolver == null)
            GlobalFontSettings.FontResolver = SystemFontResolver.Instance;

        bool hr   = language == "hr";
        int  year = gender == "men" ? 2018 : 2019;
        new Renderer(hr, year, teamCountry, topScorers, yellowCards, attendance).Write(filePath);
    }

    private sealed class Renderer
    {
        private const double PW   = 595;
        private const double PH   = 842;
        private const double ML   = 50;
        private const double MT   = 48;
        private const double CW   = 495;
        private const double FY   = PH - 44;
        private const double RowH = 18;

        private static readonly XColor ColHdrBg  = XColor.FromArgb(0x33, 0x33, 0x33);
        private static readonly XColor ColAltRow = XColor.FromArgb(0xF5, 0xF5, 0xF5);
        private static readonly XColor ColSep    = XColor.FromArgb(0xCC, 0xCC, 0xCC);
        private static readonly XColor ColGray   = XColor.FromArgb(0x88, 0x88, 0x88);

        private static readonly XBrush BrHdrBg  = new XSolidBrush(ColHdrBg);
        private static readonly XBrush BrAlt    = new XSolidBrush(ColAltRow);
        private static readonly XBrush BrGray   = new XSolidBrush(ColGray);
        private static readonly XPen   PenSep   = new XPen(ColSep, 0.35);
        private static readonly XPen   PenRule  = new XPen(ColGray, 0.5);

        private readonly XFont _fTitle;
        private readonly XFont _fSub;
        private readonly XFont _fSection;
        private readonly XFont _fColHead;
        private readonly XFont _fBody;
        private readonly XFont _fFooter;

        private static readonly XStringFormat FmtCL = new() { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
        private static readonly XStringFormat FmtCR = new() { Alignment = XStringAlignment.Far,  LineAlignment = XLineAlignment.Center };
        private static readonly XStringFormat FmtCC = new() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
        private static readonly XStringFormat FmtTL = new() { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Near };

        private readonly bool           _hr;
        private readonly int            _year;
        private readonly string         _country;
        private readonly List<string[]> _topScorers;
        private readonly List<string[]> _yellowCards;
        private readonly List<string[]> _attendance;

        private PdfDocument     _doc  = null!;
        private List<XGraphics> _gfxs = null!;
        private double           _y;

        private XGraphics G => _gfxs[^1];

        public Renderer(bool hr, int year, string country,
            List<string[]> ts, List<string[]> yc, List<string[]> att)
        {
            _hr           = hr;
            _year         = year;
            _country      = country;
            _topScorers   = ts;
            _yellowCards  = yc;
            _attendance   = att;

            _fTitle   = new XFont("Arial", 17, XFontStyleEx.Bold);
            _fSub     = new XFont("Arial",  9, XFontStyleEx.Regular);
            _fSection = new XFont("Arial", 11, XFontStyleEx.Bold);
            _fColHead = new XFont("Arial",  9, XFontStyleEx.Bold);
            _fBody    = new XFont("Arial",  9, XFontStyleEx.Regular);
            _fFooter  = new XFont("Arial",  8, XFontStyleEx.Regular);
        }

        public void Write(string filePath)
        {
            _doc  = new PdfDocument();
            _gfxs = new List<XGraphics>();
            AddPage();

            DrawDocHeader();

            DrawTable(
                _hr ? "Strijelci" : "Top Scorers",
                new[] { "#", _hr ? "Igrač" : "Player Name", _hr ? "Golovi" : "Goals", _hr ? "Nastupi" : "Appearances" },
                new[] { 30d, 250d, 100d, 115d },
                _topScorers);

            _y += 18;

            DrawTable(
                _hr ? "Žuti kartoni" : "Yellow Cards",
                new[] { "#", _hr ? "Igrač" : "Player Name", _hr ? "Žuti kartoni" : "Yellow Cards", _hr ? "Nastupi" : "Appearances" },
                new[] { 30d, 250d, 100d, 115d },
                _yellowCards);

            _y += 18;

            DrawTable(
                _hr ? "Posjećenost utakmica" : "Match Attendance",
                new[] { "#", _hr ? "Mjesto" : "Location", _hr ? "Posjećenost" : "Attendance", _hr ? "Domaćin" : "Home Team", _hr ? "Gost" : "Away Team" },
                new[] { 30d, 165d, 85d, 110d, 105d },
                _attendance);

            int total = _gfxs.Count;
            for (int i = 0; i < total; i++)
            {
                DrawFooter(_gfxs[i], i + 1, total);
                _gfxs[i].Dispose();
            }

            _doc.Save(filePath);
        }

        private void AddPage()
        {
            var page = _doc.AddPage();
            page.Width  = XUnit.FromMillimeter(210);
            page.Height = XUnit.FromMillimeter(297);
            _gfxs.Add(XGraphics.FromPdfPage(page));
            _y = MT;
        }

        private void EnsureSpace(double neededPts)
        {
            if (_y + neededPts > FY)
                AddPage();
        }

        private void DrawDocHeader()
        {
            string title = $"World Cup {_year} — {_country} Rankings";
            G.DrawString(title, _fTitle, XBrushes.Black, new XRect(ML, _y, CW, 22), FmtTL);
            _y += 24;

            string date = DateTime.Today.ToString("dd.MM.yyyy");
            G.DrawString(date, _fSub, BrGray, new XRect(ML, _y, CW, 14), FmtTL);
            _y += 16;

            G.DrawLine(PenRule, ML, _y, ML + CW, _y);
            _y += 14;
        }

        private void DrawTable(string title, string[] headers, double[] colW, List<string[]> rows)
        {
            EnsureSpace(22 + RowH + RowH + 2);

            G.DrawString(title, _fSection, XBrushes.Black, new XRect(ML, _y, CW, 18), FmtTL);
            _y += 20;

            DrawRow(headers, colW, isHeader: true, altRow: false);

            for (int i = 0; i < rows.Count; i++)
            {
                EnsureSpace(RowH);
                DrawRow(rows[i], colW, isHeader: false, altRow: i % 2 == 1);
            }
        }

        private void DrawRow(string[] cells, double[] colW, bool isHeader, bool altRow)
        {
            var rowRect = new XRect(ML, _y, CW, RowH);

            if (isHeader)
                G.DrawRectangle(BrHdrBg, rowRect);
            else if (altRow)
                G.DrawRectangle(BrAlt, rowRect);

            XFont  font = isHeader ? _fColHead : _fBody;
            XBrush fg   = isHeader ? XBrushes.White : XBrushes.Black;
            double x    = ML + 4;

            for (int i = 0; i < Math.Min(cells.Length, colW.Length); i++)
            {
                string txt  = i < cells.Length ? cells[i] : "";
                var    rect = new XRect(x, _y, colW[i] - 5, RowH);
                G.DrawString(txt, font, fg, rect, FmtCL);
                x += colW[i];
            }

            G.DrawLine(PenSep, ML, _y + RowH, ML + CW, _y + RowH);

            _y += RowH;
        }

        private void DrawFooter(XGraphics gfx, int pageNum, int total)
        {
            string text = _hr
                ? $"Stranica {pageNum} od {total}"
                : $"Page {pageNum} of {total}";

            gfx.DrawString(text, _fFooter, BrGray,
                new XRect(ML, FY + 10, CW, 20), FmtCR);
        }
    }
}
