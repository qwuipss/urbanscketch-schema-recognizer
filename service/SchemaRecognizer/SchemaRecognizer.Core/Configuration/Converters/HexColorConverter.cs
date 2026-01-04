using System.ComponentModel;
using System.Globalization;
using SchemaRecognizer.Core.Models;

namespace SchemaRecognizer.Core.Configuration.Converters;

public sealed class HexColorConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string);

    public override object ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value
    )
    {
        const int hexBase = 16;

        var color = (value as string)?.Trim();

        if (color?.Length is not 6
            || !color.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F'))
        {       throw new FormatException("Specified color is not valid hex color");}

        var r = Convert.ToByte(color[..2], hexBase);
        var g = Convert.ToByte(color[2..4], hexBase);
        var b = Convert.ToByte(color[4..6], hexBase);

        return new Color(r, g, b);
    }
}