using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
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

        // if (expr)
        {
        }

        return PdfPathFilterVerdict.None;
    }

    private static bool IsBoundingRectanglePresent(PdfPath path)
    {
        var boundingRectangle = path.GetBoundingRectangle();

        return boundingRectangle.HasValue;
    }

    private bool IsFillColorBlacklisted(PdfPath path)
    {
        if (!path.IsFilled)
        {
            return false;
        }

        var (r, g, b) = path.FillColor!.ToRGBValues();
        var blacklistedColors = _options.Value.ColorsBlacklist;
        return false;
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