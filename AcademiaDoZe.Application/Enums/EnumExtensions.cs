using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AcademiaDoZe.Application.Enums;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var name = field?.GetCustomAttribute<DisplayAttribute>()?.Name;
        if (!string.IsNullOrWhiteSpace(name)) return name;
        if (value.GetType().GetCustomAttribute<FlagsAttribute>() is not null)
        {
            var names = Enum.GetValues(value.GetType()).Cast<Enum>()
                .Where(flag => Convert.ToInt64(flag) != 0 && value.HasFlag(flag))
                .Select(flag => flag.GetType().GetField(flag.ToString())?.GetCustomAttribute<DisplayAttribute>()?.Name ?? flag.ToString());
            return string.Join(", ", names);
        }
        return value.ToString();
    }
}
