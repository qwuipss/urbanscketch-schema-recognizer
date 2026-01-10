using SchemaRecognizer.Core.Models;
using UglyToad.PdfPig.Core;

namespace SchemaRecognizer.Core.Configuration;

public sealed class PdfPathFilterOptions
{
    public int CommandsCountLimit { get; set; } = 20;

    public int SmallAreaThreshold { get; set; } = 100;

    public int SmallWidthThreshold { get; set; } = 10;

    public int SmallHeightThreshold { get; set; } = 10;

    public PdfRectangle? BoundingBox { get; set; }

    public List<Color> ColorsBlacklist { get; set; } = [];
}