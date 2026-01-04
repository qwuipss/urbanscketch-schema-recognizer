using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Geometry;
using UglyToad.PdfPig.Graphics;

namespace SchemaRecognizer.Core.Pdf.Filtering;

public sealed class PdfPathFilter(IOptions<PdfPathFilterOptions> options) : IPdfPathFilter
{
    private readonly IOptions<PdfPathFilterOptions> _options = options;

    public PdfPathFilterVerdict GetFilterVerdict(PdfPath path)
    {
        if (IsCommandsLimitExceeded(path))
        {
            return PdfPathFilterVerdict.CommandsLimitExceeded;
        }

        if (!IsBoundingRectanglePresent(path))
        {
            return PdfPathFilterVerdict.BoundingRectangleNotPresented;
        }

        if (!IsInBoundingBox(path))
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

    private bool IsInBoundingBox(PdfPath path)
    {
        var filterOptions = _options.Value;

        if (filterOptions.BoundingBox is null)
        {
            return true;
        }

        var boundingRectangle = path.GetBoundingRectangle();

        
        return boundingRectangle is null || filterOptions.BoundingBox.Value.IntersectsWith(boundingRectangle.Value);
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