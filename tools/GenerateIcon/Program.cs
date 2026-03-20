using System.Drawing;
using System.Drawing.Drawing2D;

// Output path relative to the tool location
var output = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "../../../../src/speech2text/Resources/app.ico"));

Directory.CreateDirectory(Path.GetDirectoryName(output)!);

using var bitmap = new Bitmap(32, 32);
using var g = Graphics.FromImage(bitmap);

g.SmoothingMode     = SmoothingMode.AntiAlias;
g.InterpolationMode = InterpolationMode.HighQualityBicubic;
g.PixelOffsetMode   = PixelOffsetMode.HighQuality;

// ── Blue circle background ──────────────────────────────────────────
using var bgBrush = new SolidBrush(Color.FromArgb(0x03, 0xA9, 0xF4));
g.FillEllipse(bgBrush, 0, 0, 31, 31);

// ── Mic body: rounded rect Rect="13,7,6,10" RadiusX="3" ────────────
using var micBrush = new SolidBrush(Color.Black);
using var micPath  = RoundedRect(13, 7, 6, 10, 3);
g.FillPath(micBrush, micPath);

// ── Mic stand, stem, base ──────────────────────────────────────────
using var pen = new Pen(Color.Black, 1.8f)
{
    StartCap = LineCap.Round,
    EndCap   = LineCap.Round,
};

// Stand: two quadratic beziers converted to cubics
// Q1: (11,17) ctrl (11,21) end (16,21)
// Q2: (16,21) ctrl (21,21) end (21,17)
using var stand = new GraphicsPath();
stand.AddBezier(11, 17, 11, 19.67f, 12.67f, 21, 16, 21);
stand.AddBezier(16, 21, 19.33f, 21, 21, 19.67f, 21, 17);
g.DrawPath(pen, stand);

// Stem
g.DrawLine(pen, 16, 21, 16, 25);

// Base
g.DrawLine(pen, 13, 25, 19, 25);

// ── Save as .ico ───────────────────────────────────────────────────
var hicon = bitmap.GetHicon();
using var icon = Icon.FromHandle(hicon);
using var fs   = File.Create(output);
icon.Save(fs);

Console.WriteLine($"Icon saved to: {output}");

// ── Helper ─────────────────────────────────────────────────────────
static GraphicsPath RoundedRect(float x, float y, float w, float h, float r)
{
    var path = new GraphicsPath();
    path.AddArc(x,         y,         2*r, 2*r, 180, 90);
    path.AddArc(x+w-2*r,  y,         2*r, 2*r, 270, 90);
    path.AddArc(x+w-2*r,  y+h-2*r,   2*r, 2*r,   0, 90);
    path.AddArc(x,         y+h-2*r,   2*r, 2*r,  90, 90);
    path.CloseFigure();
    return path;
}
