using System.ComponentModel;
using SchemaRecognizer.Core.Configuration.Converters;

namespace SchemaRecognizer.Core.Models;

[TypeConverter(typeof(HexColorConverter))]
public readonly record struct Color(byte R, byte G, byte B)
{
    public bool IsSimilarTo((double R, double G, double B) color)
    {
        const double delta = 10;

        var deltaR = Math.Abs(R - color.R * byte.MaxValue);
        var deltaG = Math.Abs(G - color.G * byte.MaxValue);
        var deltaB = Math.Abs(B - color.B * byte.MaxValue);

        return deltaR <= delta
               && deltaB <= delta
               && deltaG <= delta;
    }
}