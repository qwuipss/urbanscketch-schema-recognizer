using SchemaRecognizer.Core.Figures;

namespace SchemaRecognizer.Core.Extensions;

public static class CoordinateExtensions
{
    public static double[] ToArray(this Coordinate coordinate)
    {
        return [coordinate.X, coordinate.Y,];
    }
}