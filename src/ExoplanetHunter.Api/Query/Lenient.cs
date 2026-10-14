using System.Globalization;

namespace ExoplanetHunter.Api.Query;

/// <summary>
/// Query-string parsing with the legacy MVC model binder's tolerance: a missing or
/// malformed value falls back to the default instead of failing the request. Existing
/// clients send values such as <c>max=null</c> and <c>type=</c>.
/// </summary>
public static class Lenient
{
    public static int Int(string? value, int fallback = 0) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : fallback;

    public static int? NullableInt(string? value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : null;

    public static double? NullableDouble(string? value) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : null;

    public static bool Bool(string? value, bool fallback = false) =>
        bool.TryParse(value, out var b) ? b : fallback;

    /// <summary>Accepts a member name (case-insensitive) or its ordinal.</summary>
    public static T Enum<T>(string? value, T fallback) where T : struct, System.Enum =>
        System.Enum.TryParse<T>(value, ignoreCase: true, out var result) && System.Enum.IsDefined(result) ? result : fallback;
}
