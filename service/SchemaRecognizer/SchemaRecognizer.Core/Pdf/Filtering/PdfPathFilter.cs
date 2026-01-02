using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SkiaSharp;
using UglyToad.PdfPig.Graphics;

namespace SchemaRecognizer.Core.Pdf.Filtering;

public sealed class PdfPathFilter(IOptions<PdfPathFilterOptions> options) : IPdfPathFilter
{
    private readonly IOptions<PdfPathFilterOptions> _options = options;

    public PdfPathFilterVerdict GetFilterVerdict(PdfPath path, SKBitmap bitmap)
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

        if (IsFillColorBlacklisted(path, bitmap))
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

    private bool IsFillColorBlacklisted(PdfPath path, SKBitmap bitmap)
    {
        // if (!path.IsFilled)
        // {
        //     return false;
        // }

        var boundingRectangle = path.GetBoundingRectangle();

        if (boundingRectangle is null)
        {
            return false;
        }

        var x0 = Math.Clamp((int)Math.Floor(boundingRectangle.Value.Left), 0, bitmap.Width - 1);
        var x1 = Math.Clamp((int)Math.Ceiling(boundingRectangle.Value.Right), 0, bitmap.Width);
        var y1 = Math.Clamp((int)Math.Floor(boundingRectangle.Value.Top), 0, bitmap.Height - 1);
        var y0 = Math.Clamp((int)Math.Ceiling(boundingRectangle.Value.Bottom), 0, bitmap.Height);

        if (x1 <= x0 || y1 <= y0)
        {
            return false;
        }

        long sumR = 0, sumG = 0, sumB = 0;
        long count = 0;

        unsafe
        {
            var pixels = (SKColor*)bitmap.GetPixels();

            for (var y = y0; y < y1; y++)
            {
                var row = y * bitmap.Width;
                for (var x = x0; x < x1; x++)
                {
                    var c = pixels[row + x];

                    if (c.Alpha < 10)
                    {  continue;}

                    sumR += c.Red;
                    sumG += c.Green;
                    sumB += c.Blue;
                    count++;
                }
            }
        }

        if (count is 0)
        {
            return false;
        }

        var avgColor = (
            R: sumR / (double)count,
            G: sumG / (double)count,
            B: sumB / (double)count
        );

        foreach (var blacklistedColor in _options.Value.ColorsBlacklist)
        {
            if (blacklistedColor.IsSimilarTo(avgColor))
                return true;
        }

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