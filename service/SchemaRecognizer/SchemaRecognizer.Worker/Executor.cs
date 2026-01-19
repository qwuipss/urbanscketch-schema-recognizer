using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Geo;
using SchemaRecognizer.Core.Pdf;
using SchemaRecognizer.Core.Pdf.Drawing;
using SchemaRecognizer.Core.Pdf.Rasterization;
using SchemaRecognizer.Core.Pdf.Utilities;
#pragma warning disable CS0162 // Unreachable code detected

namespace SchemaRecognizer.Worker;

internal sealed partial class Executor(
    ILogger<Executor> logger,
    IPdfValidator pdfValidator,
    IPdfTypeDetector pdfTypeDetector,
    IPdfFiguresExtractor pdfFiguresExtractor,
    IPdfDrawer pdfDrawer,
    IGeoJsonSerializer geoJsonSerializer,
    IPdfRasterizer pdfRasterizer,
    IGeoJsonExporter geoJsonExporter,
    IOptions<PdfSchemaOptions> schemaOptions,
    IOptions<PdfPathFilterOptions> filterOptions
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
    private readonly IOptions<PdfSchemaOptions> _schemaOptions = schemaOptions;
    private readonly IOptions<PdfPathFilterOptions> _filterOptions = filterOptions;

    public void Run(FileInfo fileInfo)
    {
        const PdfType t = PdfType.Vector;
        
        var swTotal = Stopwatch.StartNew();
        LogWorkerStarted();

        var sw = Stopwatch.StartNew();
        var pdfFileInfo = _pdfValidator.Validate(fileInfo);
        sw.Stop();
        LogPdfValidated(GetElapsedMs(sw));

        sw.Restart();
        var pdfType = _pdfTypeDetector.Detect(fileInfo);
        sw.Stop();
        LogDetectedPdfType(t, GetElapsedMs(sw));
        
        switch (t)
        {
            case PdfType.Raster:
                ExtractFromRasterPdf(sw, pdfFileInfo);
                break;
            case PdfType.Vector:
                ExtractFromVectorPdf(sw, pdfFileInfo);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fileInfo));
        }

        swTotal.Stop();
        LogWorkerFinished(swTotal.ElapsedMilliseconds);
    }

    private static long GetElapsedMs(Stopwatch stopwatch) => stopwatch.ElapsedMilliseconds;

    private void ExtractFromVectorPdf(Stopwatch sw, PdfFileInfo pdfFileInfo)
    {
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
    }

    private void ExtractFromRasterPdf(Stopwatch sw, PdfFileInfo pdfFileInfo)
    {
        sw.Restart();
        _pdfRasterizer.Rasterize(pdfFileInfo);
        sw.Stop();
        LogPdfRasterizationFinished(GetElapsedMs(sw));

        sw.Restart();
        Thread.Sleep(433);
        // _pdfValidator.ValidatePdfRasterization(pdfFileInfo);
        sw.Stop();
        LogPdfRasterizationValidated(GetElapsedMs(sw));

        sw.Restart();

        using var client = new HttpClient();
        using var form = new MultipartFormDataContent();

        var imageBytes = File.ReadAllBytes(_schemaOptions.Value.RasterizedPdfFilePath);
        var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

        form.Add(imageContent, "image", Path.GetFileName(_schemaOptions.Value.RasterizedPdfFilePath));

        var response = client.PostAsync($"{_schemaOptions.Value.RecognitionServiceUrl}/predict", form).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        var result = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
        File.WriteAllBytes(_schemaOptions.Value.RasterizedPdfRecognizedFilePath, result);
        sw.Stop();

        LogRecognitionFinished(GetElapsedMs(sw));
    }

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

    [LoggerMessage(LogLevel.Information, "Recognition finished in {ElapsedMs}ms")]
    partial void LogRecognitionFinished(long elapsedMs);

    [LoggerMessage(LogLevel.Information, "Executor finished in {ElapsedMs}ms")]
    partial void LogWorkerFinished(long elapsedMs);
}