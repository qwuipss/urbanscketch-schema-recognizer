using System.Text.Json;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;
using SchemaRecognizer.Core.Configuration;

namespace SchemaRecognizer.Core.Geo;

public sealed class GeoJsonExporter(IOptions<PdfSchemaOptions> options) : IGeoJsonExporter
{
    private readonly IOptions<PdfSchemaOptions> _options = options;

    public void Export()
    {
        using var conn = new NpgsqlConnection(_options.Value.DbConnectionString);

        conn.Open();

        CreateExtensionsAndTableIfNotExists(conn);

        using var command = new NpgsqlCommand(
            """
                INSERT INTO geo_features (kind, geom)
                VALUES (@kind, ST_SetSRID(@geom, 4326))
            """,
            conn
        );

        var geoJson = File.ReadAllText(_options.Value.WriteFiguresGeoJsonFilePath);
        using var doc = JsonDocument.Parse(geoJson);
        var reader = new GeoJsonReader();

        foreach (var feature in doc.RootElement.GetProperty("features").EnumerateArray())
        {
            var geometryJson = feature.GetProperty("geometry").GetRawText();
            var geom = reader.Read<Geometry>(geometryJson);

            var kind = feature
                       .GetProperty("properties")
                       .GetProperty("kind")
                       .GetString();

            if (kind is null)
            {
                throw new ArgumentException("GeoJson has invalid format. Missed structure: features->geometry->properties->kind");
            }

            command.Parameters.Clear();
            command.Parameters.AddWithValue("kind", kind);
            command.Parameters.AddWithValue("geom", geom.AsText());

            command.ExecuteNonQuery();
        }

        conn.Close();
    }

    private static void CreateExtensionsAndTableIfNotExists(NpgsqlConnection conn)
    {
        using var command = new NpgsqlCommand(
            """
            CREATE EXTENSION IF NOT EXISTS postgis;

            CREATE TABLE IF NOT EXISTS geo_features (
                id SERIAL PRIMARY KEY,
                kind TEXT,
                geom GEOMETRY(Polygon, 4326)
            );
            """,
            conn
        );

        command.ExecuteNonQuery();
    }
}