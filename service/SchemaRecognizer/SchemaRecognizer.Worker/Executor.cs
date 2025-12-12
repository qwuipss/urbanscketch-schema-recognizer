using Microsoft.Extensions.Logging;
using SchemaRecognizer.Core.Pdf;
using SchemaRecognizer.Core.Pdf.Drawing;
using SchemaRecognizer.Core.Pdf.Utilities;
using UglyToad.PdfPig;

namespace SchemaRecognizer.Worker;

internal sealed partial class Executor(
    ILogger<Executor> logger,
    IPdfValidator pdfValidator,
    IPdfTypeDetector pdfTypeDetector,
    IPdfFiguresExtractor pdfFiguresExtractor,
    IPdfDrawer pdfDrawer
) : IExecutor
{
    private readonly ILogger<Executor> _logger = logger;
    private readonly IPdfDrawer _pdfDrawer = pdfDrawer;
    private readonly IPdfFiguresExtractor _pdfFiguresExtractor = pdfFiguresExtractor;
    private readonly IPdfTypeDetector _pdfTypeDetector = pdfTypeDetector;
    private readonly IPdfValidator _pdfValidator = pdfValidator;

    public void Run(FileInfo fileInfo)
    {
        LogWorkerStarted();
        _pdfValidator.Validate(fileInfo);
        LogPdfValidated();

        var pdfType = _pdfTypeDetector.Detect(fileInfo);
        LogDetectedPdfType(pdfType);

        if (pdfType is PdfType.Raster)
        {
            throw new NotSupportedException(); // temp
        }

        var figures = _pdfFiguresExtractor.Extract(fileInfo);
        var pageSize = GetPageSize(fileInfo);

        _pdfDrawer.Draw(figures, pageSize);
    }

    private static (double Width, double Height) GetPageSize(FileInfo fileInfo)
    {
        using var doc = PdfDocument.Open(fileInfo.FullName);
        var page = doc.GetPages().Single();

        return (page.Width, page.Height);
    }

    [LoggerMessage(LogLevel.Information, "Executor started")]
    partial void LogWorkerStarted();

    [LoggerMessage(LogLevel.Information, "Pdf validated")]
    partial void LogPdfValidated();

    [LoggerMessage(LogLevel.Information, "Detected pdf type: {pdfType}")]
    partial void LogDetectedPdfType(PdfType pdfType);
}