namespace SchemaRecognizer.Core.Configuration;

public sealed class PdfPathFilterOptions
{
    public int CommandsCountLimit { get; set; } = 20;

    public int SmallAreaThreshold { get; set; } = 100;

    public int SmallWidthThreshold { get; set; } = 10;

    public int SmallHeightThreshold { get; set; } = 10;

    public List<Color> ColorsBlacklist { get; set; } = [];
}