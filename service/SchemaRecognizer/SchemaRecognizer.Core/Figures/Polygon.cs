using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using SchemaRecognizer.Core.Pdf;
using UglyToad.PdfPig.Core;
using static SchemaRecognizer.Core.Pdf.Constants;

namespace SchemaRecognizer.Core.Figures;

public sealed class Polygon(PdfSubpath subPath) : Figure
{
    private readonly List<Coordinate> _coordinates = GetCoordinates(subPath);

    public override void Draw(PdfCanvas canvas)
    {
        if (_coordinates.Count is 0)
        {
            return;
        }

        canvas.SetLineWidth(1);
        canvas.SetFillColorRgb(0, .25f, .5f);
        canvas.SetStrokeColorRgb(0, .8f, .8f);

        canvas.MoveTo(_coordinates[0].X, _coordinates[0].Y);

        foreach (var coordinate in _coordinates.Skip(1))
        {
            canvas.LineTo(coordinate.X, coordinate.Y);
        }

        canvas.ClosePathFillStroke();

        canvas.BeginText();
        canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 5);

        canvas.MoveText(_coordinates[0].X, _coordinates[0].Y);

        canvas.ShowText("asd");
        canvas.EndText();
    }

    public override object GetGeoJsonFeature(PdfFileInfo pdfFileInfo)
    {
        var featureCoordinates = new List<double[]>();

        foreach (var coordinate in _coordinates)
        {
            var xMeters =
                coordinate.X
                / PdfMmToPtFactor
                * pdfFileInfo.Scale
                / MillimetersInMeter;

            var yMeters =
                coordinate.Y
                / PdfMmToPtFactor
                * pdfFileInfo.Scale
                / MillimetersInMeter;

            var longitude = xMeters / EarthRadiusMeters * RadiansToDegreesFactor;
            var latitude = yMeters / EarthRadiusMeters * RadiansToDegreesFactor;

            featureCoordinates.Add([longitude, latitude,]);
        }

        return new
        {
            type = "Feature",
            geometry = new
            {
                type = "Polygon",
                coordinates = new[] { featureCoordinates }
            },
            properties = new
            {
                kind = nameof(Polygon)
            }
        };
    }

    private static List<Coordinate> GetCoordinates(PdfSubpath subPath)
    {
        var hasClose = false;
        var hasLine = false;
        var hasMove = false;
        var coordinates = new List<Coordinate>(subPath.Commands.Count);

        foreach (var command in subPath.Commands)
        {
            switch (command)
            {
                case PdfSubpath.Move moveCommand:
                    hasMove = true;
                    coordinates.Add(new Coordinate(moveCommand.Location.X, moveCommand.Location.Y));
                    break;
                case PdfSubpath.Line lineCommand:
                    hasLine = true;
                    coordinates.Add(new Coordinate(lineCommand.To.X, lineCommand.To.Y));
                    break;
                case PdfSubpath.Close:
                    hasClose = true;
                    break;
                default:
                    throw new ArgumentException($"Command {command.GetType()} is not supported by {nameof(Polygon)}");
            }
        }

        if (!hasMove)
        {
            throw new ArgumentException($"{nameof(PdfSubpath)} for {nameof(Polygon)} has no {nameof(PdfSubpath.Move)}");
        }

        if (!hasLine)
        {
            throw new ArgumentException($"{nameof(PdfSubpath)} for {nameof(Polygon)} has no {nameof(PdfSubpath.Line)}");
        }

        if (!hasClose)
        {
            throw new ArgumentException($"{nameof(PdfSubpath)} for {nameof(Polygon)} has no  {nameof(PdfSubpath.Close)}");
        }

        return coordinates;
    }
}