using SkiaSharp;
using UglyToad.PdfPig.Graphics;

namespace SchemaRecognizer.Core.Pdf.Filtering;

public interface IPdfPathFilter
{
    PdfPathFilterVerdict GetFilterVerdict(PdfPath path);
}