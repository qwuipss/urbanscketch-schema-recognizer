using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Extensions;
using SchemaRecognizer.Core.Figures;
using SchemaRecognizer.Core.Pdf.Filtering;
using SkiaSharp;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Graphics;
using PdfDocument = UglyToad.PdfPig.PdfDocument;

namespace SchemaRecognizer.Core.Pdf;

public sealed partial class PdfFiguresExtractor(
    ILogger<PdfFiguresExtractor> logger,
    IPdfPathFilter pdfPathFilter,
    IOptions<PdfSchemaOptions> options
)
    : IPdfFiguresExtractor
{
    private readonly ILogger<PdfFiguresExtractor> _logger = logger;
    private readonly IPdfPathFilter _pdfPathFilter = pdfPathFilter;
    private readonly IOptions<PdfSchemaOptions> _options = options;

    public ICollection<Figure> Extract(PdfFileInfo pdfFileInfo)
    {
        using var document = PdfDocument.Open(pdfFileInfo.FileInfo.FullName);
        var page = document.GetPages().Single();
        var filterVerdictStatistics = GetFilterVerdictStatistics();
        var figures = new List<Figure>();

        foreach (var path in page.Paths)
        {
            var filterVerdict = _pdfPathFilter.GetFilterVerdict(path, pdfFileInfo);

            filterVerdictStatistics[filterVerdict]++;

            if (filterVerdict is not PdfPathFilterVerdict.None)
            {
                continue;
            }

            if (IsRedLine(path))
            {
                continue;
            }

            foreach (var subPath in path)
            {
                if (subPath.IsClosed() && subPath.HasClose() && !subPath.HasBezierCurve())
                {
                    var polygon = new Polygon(subPath);
                    figures.Add(polygon);
                }
            }
        }

        LogFilterVerdictStatistics(filterVerdictStatistics);

        return figures;
    }

    private bool IsRedLine(PdfPath path)
    {
        if (path is not { IsFilled: true, FillColor: not null })
        {
            return false;
        }

        if (path.Any(subPath => !subPath.IsClosed() || !subPath.HasClose()))
        {
            return false;
        }

        var redLineThreshold = _options.Value.RedLineThreshold;
        var fillColor = path.FillColor.ToRGBValues();

        return fillColor.r * byte.MaxValue > redLineThreshold.R
               && fillColor.g * byte.MaxValue < redLineThreshold.G
               && fillColor.b * byte.MaxValue < redLineThreshold.B;
    }

    private void LogFilterVerdictStatistics(Dictionary<PdfPathFilterVerdict, int> filterVerdictStatistics)
    {
        const int paddingWidth = 2;

        var stringBuilder = new StringBuilder();
        var nameMaxWidth = filterVerdictStatistics.Keys.Max(filterVerdict => filterVerdict.ToString().Length) + paddingWidth;
        var valueMaxWidth = filterVerdictStatistics.Values.Max(count => count.ToString().Length) + paddingWidth;

        stringBuilder.AppendLine("Filter verdict statistics:");

        foreach (var (filterVerdict, count) in filterVerdictStatistics)
        {
            stringBuilder.AppendLine(
                $"\t{filterVerdict.ToString().PadRight(nameMaxWidth)}{count.ToString().PadLeft(valueMaxWidth)}"
            );
        }

        var logMessage = stringBuilder.ToString();

        LogFilterVerdictStatistics(logMessage);
    }

    private static Dictionary<PdfPathFilterVerdict, int> GetFilterVerdictStatistics()
    {
        var filterVerdictCounters = new Dictionary<PdfPathFilterVerdict, int>();

        foreach (var filterVerdict in Enum.GetValues<PdfPathFilterVerdict>())
        {
            filterVerdictCounters[filterVerdict] = 0;
        }

        return filterVerdictCounters;
    }

    [LoggerMessage(LogLevel.Information, "{FilterVerdictStatistics}")]
    partial void LogFilterVerdictStatistics(string filterVerdictStatistics);
}