namespace SchemaRecognizer.Core.Helpers;

public static class MathHelper
{
    public static bool AreEqual(double first, double second, double epsilon = 10e-4)
    {
        return Math.Abs(first - second) < epsilon;
    }
}