using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Geometry;
using UglyToad.PdfPig.Graphics;

namespace SchemaRecognizer.Core.Pdf.Filtering;

public sealed class PdfPathFilter(IOptions<PdfPathFilterOptions> options) : IPdfPathFilter
{
    private readonly IOptions<PdfPathFilterOptions> _options = options;

    public PdfPathFilterVerdict GetFilterVerdict(PdfPath path, PdfFileInfo pdfFileInfo)
    {
        if (IsCommandsLimitExceeded(path))
        {
            return PdfPathFilterVerdict.CommandsLimitExceeded;
        }

        if (!IsBoundingRectanglePresent(path))
        {
            return PdfPathFilterVerdict.BoundingRectangleNotPresented;
        }

        if (!IsInBoundingBox(path, pdfFileInfo))
        {
            return PdfPathFilterVerdict.OutOfBoundingBox;
        }

        if (IsBoundingRectangleSmallArea(path))
        {
            return PdfPathFilterVerdict.BoundingRectangleSmallArea;
        }

        if (IsBoundingRectangleSmallWidth(path))
        {
            return PdfPathFilterVerdict.BoundingRectangleSmallWidth;
        }

        if (IsBoundingRectangleSmallHeight(path))
        {
            return PdfPathFilterVerdict.BoundingRectangleSmallHeight;
        }

        if (IsFillColorBlacklisted(path))
        {
            return PdfPathFilterVerdict.ColorBlacklisted;
        }

        return PdfPathFilterVerdict.None;
    }

    private static bool IsBoundingRectanglePresent(PdfPath path)
    {
        var boundingRectangle = path.GetBoundingRectangle();

        return boundingRectangle.HasValue;
    }

    private bool IsInBoundingBox(PdfPath path, PdfFileInfo pdfFileInfo)
    {
        var filterOptions = _options.Value;
        var boundingBox = filterOptions.BoundingBox;
        var boundingRectangle = path.GetBoundingRectangle();

        if (boundingBox is null || boundingRectangle is null)
        {
            return true;
        }

        var pageHeight = pdfFileInfo.Height;
        var invertedBoundingRectangle = new PdfRectangle(
            boundingRectangle.Value.Left,
            pageHeight - boundingRectangle.Value.Top - boundingRectangle.Value.Height,
            boundingRectangle.Value.Right,
            pageHeight - boundingRectangle.Value.Top
        );

        return invertedBoundingRectangle.IntersectsWith(boundingBox.Value);
    }

    private bool IsFillColorBlacklisted(PdfPath path)
    {
        if (!path.IsFilled || path.FillColor is null)
        {
            return false;
        }

        var color = path.FillColor.ToRGBValues();

        return _options.Value.ColorsBlacklist.Any(blacklistedColor => blacklistedColor.IsSimilarTo(color));
    }

    private bool IsBoundingRectangleSmallArea(PdfPath path)
    {
        var boundingRectangle = path.GetBoundingRectangle()!.Value;
        var smallAreaThreshold = _options.Value.SmallAreaThreshold;

        return boundingRectangle.Area < smallAreaThreshold;
    }

    private bool IsBoundingRectangleSmallHeight(PdfPath path)
    {
        var boundingRectangle = path.GetBoundingRectangle()!.Value;
        var smallHeightThreshold = _options.Value.SmallHeightThreshold;

        return boundingRectangle.Area < smallHeightThreshold;
    }

    private bool IsBoundingRectangleSmallWidth(PdfPath path)
    {
        var boundingRectangle = path.GetBoundingRectangle()!.Value;
        var smallWidthThreshold = _options.Value.SmallWidthThreshold;

        return boundingRectangle.Area < smallWidthThreshold;
    }

    private bool IsCommandsLimitExceeded(PdfPath path)
    {
        var commandsCountLimit = _options.Value.CommandsCountLimit;
        return path.Count > commandsCountLimit;
    }
}