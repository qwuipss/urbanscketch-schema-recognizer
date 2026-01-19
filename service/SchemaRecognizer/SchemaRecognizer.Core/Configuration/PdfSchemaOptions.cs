using SchemaRecognizer.Core.Models;

namespace SchemaRecognizer.Core.Configuration;

public sealed class PdfSchemaOptions
{
    public double Scale { get; set; } = 1000;

    public string DrawFiguresFilePath { get; set; } = "./../../../../pdf/dev/figures.pdf";

    public string WriteFiguresGeoJsonFilePath { get; set; } = "./../../../../pdf/dev/schema.geojson";

    public string RasterizedPdfFilePath { get; set; } = "./../../../../pdf/dev/raster.jpeg";
    
    public string RasterizedPdfRecognizedFilePath { get; set; } = "./../../../../pdf/dev/raster-recognized.jpeg";

    public Color RedLineThreshold { get; set; } = new(210, 25, 25);

    public string DbConnectionString { get; set; } = "Host=localhost;Port=5432;Database=gis;Username=gis;Password=gis;";
    
    public string RecognitionServiceUrl { get; set; } = "http://127.0.0.1:5000";
}