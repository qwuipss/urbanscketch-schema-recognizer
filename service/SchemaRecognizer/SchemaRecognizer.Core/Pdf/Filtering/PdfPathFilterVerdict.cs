namespace SchemaRecognizer.Core.Pdf.Filtering;

public enum PdfPathFilterVerdict
{
    None,
    CommandsLimitExceeded,
    BoundingRectangleNotPresented,
    BoundingRectangleSmallArea,
    BoundingRectangleSmallWidth,
    BoundingRectangleSmallHeight,
    ColorBlacklisted,
    OutOfBoundingBox,
}