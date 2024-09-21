using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AnimePlayerV2.Services
{
    /// <summary>
    /// Provides extension methods for enum types.
    /// </summary>
    public static class EnumExtensions
    /// <summary>
    /// Gets the display name of an enum value.
    /// </summary>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>
    /// The display name of the enum value if it has a DisplayAttribute;
    /// otherwise, returns the string representation of the enum value.
    /// </returns>
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.GetName() ?? enumValue.ToString();
        }
    }
}
