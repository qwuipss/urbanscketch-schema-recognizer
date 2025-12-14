using System.Text.Json;
using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Extensions;
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
        using var stream = fileInfo.Open(FileMode.CreateNew, FileAccess.Write);
        var features = new List<object>();

        foreach (var figure in figures)
        {
            var figureCoordinates = figure.GetCoordinates(pdfFileInfo);

            var featureCoordinates = figureCoordinates
                                     .Select(c => c.ToArray())
                                     .ToList();

            var feature = new
            {
                type = "Feature",
                geometry = new
                {
                    type = "Polygon",
                    coordinates = new[] { featureCoordinates }
                },
                properties = new
                {
                    kind = figure.GetType().Name,
                },
            };

            features.Add(feature);
        }

        var geoJsonObject = new
        {
            type = "FeatureCollection",
            features,
        };

        JsonSerializer.Serialize(
            stream,
            geoJsonObject,
            GeoJsonSerializerOptions
        );
    }
}