namespace SchemaRecognizer.Core.Helpers;

public static class MathHelper
{
    public static bool AreEqual(double first, double second)
    {
        return Math.Abs(first - second) < 10e-4;
    }
}