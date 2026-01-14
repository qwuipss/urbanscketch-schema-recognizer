using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SchemaRecognizer.Core.Geo;
using SchemaRecognizer.Core.Pdf;
using SchemaRecognizer.Core.Pdf.Drawing;
using SchemaRecognizer.Core.Pdf.Rasterization;
using SchemaRecognizer.Core.Pdf.Utilities;

namespace SchemaRecognizer.Worker;

internal sealed partial class Executor(
    ILogger<Executor> logger,
    IPdfValidator pdfValidator,
    IPdfTypeDetector pdfTypeDetector,
    IPdfFiguresExtractor pdfFiguresExtractor,
    IPdfDrawer pdfDrawer,
    IGeoJsonSerializer geoJsonSerializer,
    IPdfRasterizer pdfRasterizer,
    IGeoJsonExporter geoJsonExporter
) : IExecutor
{
    private readonly ILogger<Executor> _logger = logger;
    private readonly IPdfDrawer _pdfDrawer = pdfDrawer;
    private readonly IPdfFiguresExtractor _pdfFiguresExtractor = pdfFiguresExtractor;
    private readonly IPdfTypeDetector _pdfTypeDetector = pdfTypeDetector;
    private readonly IPdfValidator _pdfValidator = pdfValidator;
    private readonly IGeoJsonSerializer _geoJsonSerializer = geoJsonSerializer;
    private readonly IPdfRasterizer _pdfRasterizer = pdfRasterizer;
    private readonly IGeoJsonExporter _geoJsonExporter = geoJsonExporter;

    public void Run(FileInfo fileInfo)
    {
        var swTotal = Stopwatch.StartNew();
        LogWorkerStarted();

        var sw = Stopwatch.StartNew();
        var pdfFileInfo = _pdfValidator.Validate(fileInfo);
        sw.Stop();
        LogPdfValidated(GetElapsedMs(sw));

        sw.Restart();
        var pdfType = _pdfTypeDetector.Detect(fileInfo);
        sw.Stop();
        LogDetectedPdfType(pdfType, GetElapsedMs(sw));

        if (pdfType is PdfType.Raster)
        {
            sw.Restart();
            _pdfRasterizer.Rasterize(pdfFileInfo);
            sw.Stop();
            LogPdfRasterizationFinished(GetElapsedMs(sw));

            sw.Restart();
            _pdfValidator.ValidatePdfRasterization(pdfFileInfo);
            sw.Stop();
            LogPdfRasterizationValidated(GetElapsedMs(sw));

            throw new NotSupportedException(); // temp
        }

        sw.Restart();
        var figures = _pdfFiguresExtractor.Extract(pdfFileInfo);
        sw.Stop();
        LogFiguresExtractingFinished(figures.Count, GetElapsedMs(sw));

        sw.Restart();
        _pdfDrawer.Draw(figures, pdfFileInfo);
        sw.Stop();
        LogFiguresDrawingFinished(GetElapsedMs(sw));

        sw.Restart();
        _geoJsonSerializer.Serialize(figures, pdfFileInfo);
        sw.Stop();
        LogFiguresSerializingFinished(GetElapsedMs(sw));

        sw.Restart();
        _geoJsonExporter.Export();
        sw.Stop();
        LogGeoJsonExportedToDatabase(GetElapsedMs(sw));

        swTotal.Stop();
        LogWorkerFinished(swTotal.ElapsedMilliseconds);
    }

    private static long GetElapsedMs(Stopwatch stopwatch) => stopwatch.ElapsedMilliseconds;

    [LoggerMessage(LogLevel.Information, "Executor started")]
    partial void LogWorkerStarted();

    [LoggerMessage(LogLevel.Information, "Pdf validated in {ElapsedMs}ms")]
    partial void LogPdfValidated(long elapsedMs);

    [LoggerMessage(LogLevel.Information, "Detected pdf type: {PdfType}. Detected in {ElapsedMs}ms")]
    partial void LogDetectedPdfType(PdfType pdfType, long elapsedMs);

    [LoggerMessage(
        LogLevel.Information,
        "Figures extracting finished in {ElapsedMs}ms. Extracted figures: {ExtractedFiguresCount}"
    )]
    partial void LogFiguresExtractingFinished(int extractedFiguresCount, long elapsedMs);

    [LoggerMessage(LogLevel.Information, "Figures drawing finished in {ElapsedMs}ms")]
    partial void LogFiguresDrawingFinished(long elapsedMs);

    [LoggerMessage(LogLevel.Information, "Figures serializing finished in {ElapsedMs}ms")]
    partial void LogFiguresSerializingFinished(long elapsedMs);

    [LoggerMessage(LogLevel.Information, "Pdf rasterization finished in {ElapsedMs}ms")]
    partial void LogPdfRasterizationFinished(long elapsedMs);

    [LoggerMessage(LogLevel.Information, "Pdf rasterization validated in {ElapsedMs}ms")]
    partial void LogPdfRasterizationValidated(long elapsedMs);

    [LoggerMessage(LogLevel.Information, "GeoJson exported to database in {ElapsedMs}ms")]
    partial void LogGeoJsonExportedToDatabase(long elapsedMs);

    [LoggerMessage(LogLevel.Information, "Executor finished in {ElapsedMs}ms")]
    partial void LogWorkerFinished(long elapsedMs);
}