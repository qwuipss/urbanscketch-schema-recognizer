using System.Text.Json;
using Microsoft.Extensions.Logging;
using SchemaRecognizer.Core.Geo;
using SchemaRecognizer.Core.Pdf;
using SchemaRecognizer.Core.Pdf.Drawing;
using SchemaRecognizer.Core.Pdf.Utilities;

namespace SchemaRecognizer.Worker;

internal sealed partial class Executor(
    ILogger<Executor> logger,
    IPdfValidator pdfValidator,
    IPdfTypeDetector pdfTypeDetector,
    IPdfFiguresExtractor pdfFiguresExtractor,
    IPdfDrawer pdfDrawer,
    IGeoJsonSerializer geoJsonSerializer
) : IExecutor
{
    private readonly ILogger<Executor> _logger = logger;
    private readonly IPdfDrawer _pdfDrawer = pdfDrawer;
    private readonly IPdfFiguresExtractor _pdfFiguresExtractor = pdfFiguresExtractor;
    private readonly IPdfTypeDetector _pdfTypeDetector = pdfTypeDetector;
    private readonly IPdfValidator _pdfValidator = pdfValidator;
    private readonly IGeoJsonSerializer _geoJsonSerializer = geoJsonSerializer;

    public void Run(FileInfo fileInfo)
    {
        LogWorkerStarted();

        var pdfFileInfo = _pdfValidator.Validate(fileInfo);
        LogPdfValidated();

        var pdfType = _pdfTypeDetector.Detect(fileInfo);
        LogDetectedPdfType(pdfType);

        if (pdfType is PdfType.Raster)
        {
            throw new NotSupportedException(); // temp
        }

        var figures = _pdfFiguresExtractor.Extract(pdfFileInfo);
        LogFiguresExtractingFinished(figures.Count);

        _pdfDrawer.Draw(figures, pdfFileInfo);
        LogFiguresDrawingFinished();

        _geoJsonSerializer.Serialize(figures, pdfFileInfo);
        LogFiguresSerializingFinished();
    }

    [LoggerMessage(LogLevel.Information, "Executor started")]
    partial void LogWorkerStarted();

    [LoggerMessage(LogLevel.Information, "Pdf validated")]
    partial void LogPdfValidated();

    [LoggerMessage(LogLevel.Information, "Detected pdf type: {PdfType}")]
    partial void LogDetectedPdfType(PdfType pdfType);

    [LoggerMessage(LogLevel.Information, "Figures extracting finished. Extracted figures: {ExtractedFiguresCount}")]
    partial void LogFiguresExtractingFinished(int extractedFiguresCount);

    [LoggerMessage(LogLevel.Information, "Figures drawing finished")]
    partial void LogFiguresDrawingFinished();

    [LoggerMessage(LogLevel.Information, "Figures serializing finished")]
    partial void LogFiguresSerializingFinished();
}