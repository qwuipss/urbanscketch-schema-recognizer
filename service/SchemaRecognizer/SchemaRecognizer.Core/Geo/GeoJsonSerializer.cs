using System.Text.Json;
using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Figures;
using SchemaRecognizer.Core.Helpers;
using SchemaRecognizer.Core.Pdf;

namespace SchemaRecognizer.Core.Geo;

public sealed class GeoJsonSerializer(IOptions<PdfSchemaOptions> options) : IGeoJsonSerializer
{
    private static readonly JsonSerializerOptions GeoJsonSerializerOptions =
        new()
        {
            WriteIndented = true
        };

    private readonly IOptions<PdfSchemaOptions> _options = options;

    public void Serialize(IEnumerable<Figure> figures, PdfFileInfo pdfFileInfo)
    {
        var fileInfo = FilesHelper.GetFileInfoByPath(_options.Value.WriteFiguresGeoJsonFilePath);
        var features = figures.Select(figure => figure.GetGeoJsonFeature(pdfFileInfo)).ToList();

        var geoJsonObject = new
        {
            type = "FeatureCollection",
            features,
        };

        using var stream = fileInfo.Open(FileMode.CreateNew, FileAccess.Write);

        JsonSerializer.Serialize(
            stream,
            geoJsonObject,
            GeoJsonSerializerOptions
        );
    }
}