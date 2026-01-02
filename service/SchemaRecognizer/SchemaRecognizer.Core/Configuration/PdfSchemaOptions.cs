namespace SchemaRecognizer.Core.Configuration;

public sealed class PdfSchemaOptions
{
    public double Scale { get; set; } = 1000;
    
    public string DrawFiguresFilePath { get; set; } = "./../../../../pdf/dev/figures.pdf";
    
    public string WriteFiguresGeoJsonFilePath { get; set; } = "./../../../../pdf/dev/schema.geojson";
    
    public string RasterPdfFilePath { get; set; } = "./../../../../pdf/dev/raster.jpeg";
}