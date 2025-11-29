using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using UglyToad.PdfPig.Core;

namespace SchemaRecognizer.Debug;

internal static class PdfCreator
{
    public static void Create(string filePath)
    {
        using var writer = new PdfWriter(filePath);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf, PageSize.A4);

        document.Add(
            new Paragraph("Plan")
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER)
        );

        var canvas = new PdfCanvas(pdf.GetFirstPage());

        canvas
            .SetStrokeColor(ColorConstants.RED)
            .SetLineDash(5, 5)
            .SetLineWidth(1);
        canvas.Rectangle(50, 500, 500, 300);
        canvas.Stroke();

        canvas
            .SetStrokeColor(ColorConstants.BLACK)
            .SetFillColor(new DeviceRgb(0.8f, 0.8f, 0.8f))
            .SetLineWidth(0.5f);

        canvas.Rectangle(100, 600, 120, 80);
        canvas.FillStroke();

        canvas.Rectangle(250, 550, 150, 100);
        canvas.FillStroke();

        canvas.SetFillColor(new DeviceRgb(0.7f, 0.7f, 0.7f));
        canvas.MoveTo(400, 650);
        canvas.LineTo(450, 700);
        canvas.LineTo(500, 670);
        canvas.LineTo(480, 620);
        canvas.LineTo(400, 650);
        canvas.ClosePath();
        canvas.FillStroke();

        canvas
            .SetFillColor(new DeviceRgb(0, 0.8f, 0))
            .SetStrokeColor(ColorConstants.GREEN);
        canvas.Circle(200, 400, 40);
        canvas.FillStroke();

        canvas
            .SetFillColor(new DeviceRgb(0.2f, 0.4f, 0.8f))
            .SetStrokeColor(ColorConstants.BLUE);
        canvas.Rectangle(350, 380, 120, 60);
        canvas.FillStroke();

        canvas
            .SetStrokeColor(ColorConstants.RED)
            .SetLineWidth(1.5f)
            .SetLineDash(0);
        canvas.MoveTo(50, 450);
        canvas.LineTo(550, 450);
        canvas.Stroke();

        canvas
            .BeginText()
            .SetFontAndSize(PdfFontFactory.CreateFont(), 10)
            .MoveText(110, 590)
            .ShowText("Section 1 (9 fl)")
            .MoveText(150, 0)
            .ShowText("Section 2 (12 fl)")
            .EndText();

        document.Close();
    }

    public static void Create(string filePath, IEnumerable<PdfSubpath> subPaths)
    {
        using var writer = new PdfWriter(filePath);
        using var pdf = new PdfDocument(writer);
        // var document = new Document(pdf, PageSize.A4);
        var document = new Document(pdf, new PageSize(1684, 1191));

        document.Add(
            new Paragraph("Pl2an")
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER)
        );
        var canvas = new PdfCanvas(pdf.GetFirstPage());

        foreach (var subPath in subPaths)
        {
            canvas.SetFillColor(new DeviceRgb(0.7f, 0.7f, 0.7f));

            foreach (var command in subPath.Commands)
            {
                switch (command)
                {
                    case PdfSubpath.Move moveCommand:
                        canvas.MoveTo(moveCommand.Location.X, moveCommand.Location.Y);
                        break;
                    case PdfSubpath.Line lineCommand:
                        canvas.LineTo(lineCommand.To.X, lineCommand.To.Y);
                        break;
                    case PdfSubpath.Close:
                        canvas.ClosePath();
                        break;
                    // case PdfSubpath.BezierCurve bezierCurveCommand:
                    //     canvas.CurveTo(
                    //         bezierCurveCommand.StartPoint.X,
                    //         bezierCurveCommand.StartPoint.Y,
                    //         bezierCurveCommand.EndPoint.X,
                    //         bezierCurveCommand.EndPoint.Y
                    //     );
                    //     break;
                    // default:
                    // {
                    //     if (command is PdfSubpath.CubicBezierCurve cubicBezierCurveCommand)
                    //     {
                    //         canvas.CurveTo(
                    //             cubicBezierCurveCommand.FirstControlPoint.X,
                    //             cubicBezierCurveCommand.FirstControlPoint.Y,
                    //             cubicBezierCurveCommand.SecondControlPoint.X,
                    //             cubicBezierCurveCommand.SecondControlPoint.Y,
                    //             cubicBezierCurveCommand.EndPoint.X,
                    //             cubicBezierCurveCommand.EndPoint.Y
                    //         );
                    //     }
                    //
                    //     break;
                    // }
                }

                // canvas.EndPath();
            }

            canvas.Stroke();
        }

        document.Close();
    }
}