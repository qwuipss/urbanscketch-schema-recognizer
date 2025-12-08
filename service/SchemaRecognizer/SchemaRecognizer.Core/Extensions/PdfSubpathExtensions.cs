using UglyToad.PdfPig.Core;

namespace SchemaRecognizer.Core.Extensions;

public static class PdfSubpathExtensions
{
    extension(PdfSubpath subPath)
    {
        public bool HasClose()
        {
            foreach (var command in subPath.Commands)
            {
                var commandType = command.GetType();
                if (commandType == typeof(PdfSubpath.Close))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasBezierCurve()
        {
            foreach (var command in subPath.Commands)
            {
                var commandType = command.GetType();
                if (commandType == typeof(PdfSubpath.BezierCurve)
                    || commandType == typeof(PdfSubpath.CubicBezierCurve)
                    || commandType == typeof(PdfSubpath.QuadraticBezierCurve))
                {
                    return true;
                }
            }

            return false;
        }
    }
}