namespace SchemaRecognizer.Core.Extensions;

public static class TupleExtensions
{
    public static string ToHexColorString(this (double r, double g, double b) tuple)
    {
        var rInt = (int)Math.Round(tuple.r * byte.MaxValue);
        var gInt = (int)Math.Round(tuple.g * byte.MaxValue);
        var bInt = (int)Math.Round(tuple.b * byte.MaxValue);

        return $"{rInt:X2}{gInt:X2}{bInt:X2}";
    }
}