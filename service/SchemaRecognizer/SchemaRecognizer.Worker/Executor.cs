using Microsoft.Extensions.Logging;
using SchemaRecognizer.Core.Pdf;

namespace SchemaRecognizer.Worker;

internal sealed partial class Executor(
    ILogger<Executor> logger,
    IPdfValidator pdfValidator,
    IPdfTypeDetector pdfTypeDetector
) : IExecutor
{
    private readonly ILogger<Executor> _logger = logger;
    private readonly IPdfValidator _pdfValidator = pdfValidator;
    private readonly IPdfTypeDetector _pdfTypeDetector = pdfTypeDetector;

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

        var d = new GeometryExtractor().Extract(fileInfo);
    }

    [LoggerMessage(LogLevel.Information, "Executor started")]
    partial void LogWorkerStarted();

    [LoggerMessage(LogLevel.Information, "Pdf validated")]
    partial void LogPdfValidated();

    [LoggerMessage(LogLevel.Information, "Detected pdf type: {pdfType}")]
    partial void LogDetectedPdfType(PdfType pdfType);
}