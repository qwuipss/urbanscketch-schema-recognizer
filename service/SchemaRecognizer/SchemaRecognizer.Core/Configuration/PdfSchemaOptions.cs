namespace SchemaRecognizer.Core.Configuration;

public sealed class PdfSchemaOptions
{
    public double Scale { get; init; } = 1000;
    
    public string DrawFiguresFilePath { get; init; } = "./../../../../pdf/dev/figures.pdf";
    
    public string WriteFiguresGeoJsonFilePath { get; init; } = "./../../../../pdf/dev/schema.geojson";
    
    public string RasterPdfFilePath { get; init; } = "./../../../../pdf/dev/raster.jpeg";
}