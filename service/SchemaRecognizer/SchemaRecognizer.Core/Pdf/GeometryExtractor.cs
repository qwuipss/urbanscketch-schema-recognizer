using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace SchemaRecognizer.Core.Pdf;

public class VectorShape
{
    public string Operation { get; set; } // Stroke, Fill, FillStroke

    public string ColorStroke { get; set; }

    public string ColorFill { get; set; }

    public float LineWidth { get; set; }

    public List<float[]> Points { get; set; } = new();
}

public class VectorShapeCollector : IEventListener
{
    public List<VectorShape> Shapes { get; } = new();

    public void EventOccurred(IEventData data, EventType type)
    {
        if (type != EventType.RENDER_PATH) return;

        var info = (PathRenderInfo)data;
        var path = info.GetPath();
        var op = info.GetOperation();

        var shape = new VectorShape
        {
            Operation = op.ToString(),
            LineWidth = info.GetGraphicsState().GetLineWidth()
        };

        // Stroke color
        var strokeColor = info.GetGraphicsState().GetStrokeColor();
        if (strokeColor != null)
            shape.ColorStroke = strokeColor.ToString()!;

        // Fill color
        var fillColor = info.GetGraphicsState().GetFillColor();
        if (fillColor != null)
            shape.ColorFill = fillColor.ToString()!;

        // // Points
        // foreach (var segment in path.GetPathSegments())
        // {
        //     foreach (var p in segment.GetBasePoints())
        //     {
        //         shape.Points.Add(new float[] { p.Get(0), p.Get(1) });
        //     }
        // }

        Shapes.Add(shape);
    }

    public ICollection<EventType> GetSupportedEvents() => null!;
}