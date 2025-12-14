using iText.Kernel.Pdf.Canvas;
using SchemaRecognizer.Core.Pdf;
using UglyToad.PdfPig.Core;

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
    }

    public override IEnumerable<Coordinate> GetCoordinates(PdfFileInfo pdfFileInfo)
    {
        foreach (var coordinate in _coordinates)
        {
            yield return new Coordinate(
                coordinate.X / Constants.PdfMmToPtFactor * pdfFileInfo.Scale / Constants.MillimetersInMeter,
                (pdfFileInfo.Height - coordinate.Y) / Constants.PdfMmToPtFactor * pdfFileInfo.Scale / Constants.MillimetersInMeter
            );
        }
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