using SchemaRecognizer.Core.Configuration.Validation;

namespace SchemaRecognizer.Core.Configuration;

public sealed class PdfPathFilterOptions
{
    public int CommandsCountLimit { get; init; } = 20;

    public int SmallAreaThreshold { get; init; } = 100;

    public int SmallWidthThreshold { get; init; } = 10;

    public int SmallHeightThreshold { get; init; } = 10;

    [HexColors]
    public List<string> ColorsBlacklist { get; init; } = ["FFFFFF",];
}