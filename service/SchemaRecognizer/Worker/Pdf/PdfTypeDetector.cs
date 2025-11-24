using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Worker.Pdf;

internal static class PdfTypeDetector
{
    public enum PdfType
    {
        Vector,
        Raster
    }

    public static PdfType Detect(string path)
    {
        using var pdf = new PdfReader(path);
        using var doc = new PdfDocument(pdf);

        var hasVector = false;
        var hasImage = false;

        for (int i = 1; i <= doc.GetNumberOfPages(); i++)
        {
            var page = doc.GetPage(i);

            var listener = new MyRenderListener();
            PdfCanvasProcessor parser = new PdfCanvasProcessor(listener);
            parser.ProcessPageContent(page);

            if (listener.HasVector) hasVector = true;
            if (listener.HasImage) hasImage = true;

            if (hasVector) return PdfType.Vector;
        }

        return hasImage ? PdfType.Raster : PdfType.Vector;
    }

    private class MyRenderListener : IEventListener
    {
        public bool HasVector { get; private set; } = false;

        public bool HasImage { get; private set; } = false;

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_PATH)
            {
                HasVector = true;
            }
            else if (type == EventType.RENDER_IMAGE)
            {
                HasImage = true;
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return null; // подписка на все события
        }
    }
}