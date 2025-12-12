using System.ComponentModel.DataAnnotations;

namespace SchemaRecognizer.Core.Configuration.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class HexColorsAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not List<string> colorStrings)
        {
            return false;
        }

        foreach (var colorString in colorStrings)
        {
            if (colorString.Length is not 6)
            {
                return false;
            }

            if (!IsHexColor(colorString))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsHexColor(string colorString)
    {
        foreach (var c in colorString)
        {
            var isHex = c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';

            if (!isHex)
            {
                return false;
            }
        }

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} is not valid hex colors list";
    }
}