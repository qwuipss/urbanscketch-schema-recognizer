using System.ComponentModel;

namespace SchemaRecognizer.Core.Configuration;

[TypeConverter(typeof(HexColorConverter))]
public readonly record struct Color(byte R, byte G, byte B)
{
    public bool IsSimilarTo((double R, double G, double B) color)
    {
        const double delta = 10;

        var deltaR = Math.Abs(R - color.R);
        var deltaG = Math.Abs(G - color.G);
        var deltaB = Math.Abs(B - color.B);

        return deltaR <= delta
               && deltaB <= delta
               && deltaG <= delta;
    }
}