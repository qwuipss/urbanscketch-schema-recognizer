using iText.Kernel.Pdf.Canvas;
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

    private static List<Coordinate> GetCoordinates(PdfSubpath subPath)
    {
        var isClosed = false;
        var isMoved = false;
        var coordinates = new List<Coordinate>(subPath.Commands.Count);

        foreach (var command in subPath.Commands)
        {
            switch (command)
            {
                case PdfSubpath.Move moveCommand:
                    isMoved = true;
                    coordinates.Add(new Coordinate(moveCommand.Location.X, moveCommand.Location.Y));
                    break;
                case PdfSubpath.Line lineCommand:
                    coordinates.Add(new Coordinate(lineCommand.To.X, lineCommand.To.Y));
                    break;
                case PdfSubpath.Close:
                    isClosed = true;
                    break;
                default:
                    throw new ArgumentException($"Command {command.GetType()} is not supported by {nameof(Polygon)}");
            }
        }

        if (!isMoved)
        {
            throw new ArgumentException($"{nameof(PdfSubpath)} for {nameof(Polygon)} is not moved");
        }

        if (!isClosed)
        {
            throw new ArgumentException($"{nameof(PdfSubpath)} for {nameof(Polygon)} is not closed");
        }

        return coordinates;
    }
}