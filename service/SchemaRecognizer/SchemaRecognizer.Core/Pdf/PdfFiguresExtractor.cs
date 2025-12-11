using System.Text;
using Microsoft.Extensions.Logging;
using SchemaRecognizer.Core.Extensions;
using SchemaRecognizer.Core.Figures;
using SchemaRecognizer.Core.Pdf.Filtering;
using UglyToad.PdfPig;

namespace SchemaRecognizer.Core.Pdf;

public sealed partial class PdfFiguresExtractor(ILogger<PdfFiguresExtractor> logger, IPdfPathFilter pdfPathFilter)
    : IPdfFiguresExtractor
{
    private readonly ILogger<PdfFiguresExtractor> _logger = logger;
    private readonly IPdfPathFilter _pdfPathFilter = pdfPathFilter;

    public ICollection<Figure> Extract(FileInfo fileInfo)
    {
        using var doc = PdfDocument.Open(fileInfo.FullName);
        var page = doc.GetPages().Single();
        var filterVerdictStatistics = GetFilterVerdictStatistics();
        var figures = new List<Figure>();

        foreach (var path in page.Paths)
        {
            var filterVerdict = _pdfPathFilter.GetFilterVerdict(path);

            filterVerdictStatistics[filterVerdict]++;

            if (filterVerdict is not PdfPathFilterVerdict.None)
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