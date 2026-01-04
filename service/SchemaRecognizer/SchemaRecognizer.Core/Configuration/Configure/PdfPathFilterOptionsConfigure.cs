using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UglyToad.PdfPig.Core;

namespace SchemaRecognizer.Core.Configuration.Configure;

public class PdfPathFilterOptionsConfigure(IConfiguration config) : IConfigureOptions<PdfPathFilterOptions>
{
    private readonly IConfiguration _config = config;

    public void Configure(PdfPathFilterOptions options)
    {
        var value = _config[nameof(PdfPathFilterOptions.BoundingBox)];

        if (value is not null)
            options.BoundingBox = ParseBoundingBox(value);
    }

    private static PdfRectangle ParseBoundingBox(string value)
    {
        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts?.Length is not 4)
        {
            throw new FormatException("Specified bounding box has invalid format");
        }

        var x = double.Parse(parts[0]);
        var y = double.Parse(parts[1]);
        var width = double.Parse(parts[2]);
        var height = double.Parse(parts[3]);

        if (x < 0 || y < 0 || width < 0 || height < 0)
        {
            throw new FormatException("Specified bounding box has invalid format");
        }

        return new PdfRectangle(x, y, x + width, y + height);
    }
}