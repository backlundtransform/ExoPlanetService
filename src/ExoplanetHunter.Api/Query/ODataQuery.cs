using System.Collections.Concurrent;
using System.Reflection;

namespace ExoplanetHunter.Api.Query;

/// <summary>
/// The subset of OData query options the legacy list endpoints honoured in practice:
/// $top, $skip and $orderby. In production $filter, $select and $orderby failed and
/// returned an empty 204, so no client can depend on them. $orderby is supported here;
/// any other $-option is rejected with 400 rather than silently ignored.
/// </summary>
public sealed record ODataQuery(int? Top, int? Skip, IReadOnlyList<(string Property, bool Descending)> OrderBy)
{
    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase) { "$top", "$skip", "$orderby" };
    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> Properties = new();

    public static bool TryParse(IQueryCollection query, out ODataQuery result, out string? error)
    {
        result = new ODataQuery(null, null, []);
        error = null;

        var unsupported = query.Keys.Where(k => k.StartsWith('$') && !Supported.Contains(k)).ToArray();
        if (unsupported.Length > 0)
        {
            error = $"Unsupported query option(s): {string.Join(", ", unsupported)}. Supported: $top, $skip, $orderby.";
            return false;
        }

        int? top = null, skip = null;
        if (query.TryGetValue("$top", out var topValue) && !TryParseCount(topValue, out top))
        {
            error = "$top must be a non-negative integer.";
            return false;
        }

        if (query.TryGetValue("$skip", out var skipValue) && !TryParseCount(skipValue, out skip))
        {
            error = "$skip must be a non-negative integer.";
            return false;
        }

        var orderBy = new List<(string, bool)>();
        if (query.TryGetValue("$orderby", out var orderValue))
        {
            foreach (var clause in orderValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var parts = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 2 || (parts.Length == 2 && parts[1] is not ("asc" or "desc")))
                {
                    error = $"Invalid $orderby clause '{clause}'. Use '<property> [asc|desc]'.";
                    return false;
                }

                orderBy.Add((parts[0], parts.Length == 2 && parts[1] == "desc"));
            }
        }

        result = new ODataQuery(top, skip, orderBy);
        return true;
    }

    public bool TryApply<T>(IEnumerable<T> source, out IEnumerable<T> result, out string? error)
    {
        result = source;
        error = null;

        IOrderedEnumerable<T>? ordered = null;
        foreach (var (name, descending) in OrderBy)
        {
            var property = Properties.GetOrAdd((typeof(T), name), key =>
                key.Item1.GetProperty(key.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            if (property is null || !IsSortable(property.PropertyType))
            {
                error = $"Cannot order by '{name}'.";
                return false;
            }

            Func<T, object?> key = item => property.GetValue(item);
            ordered = (ordered, descending) switch
            {
                (null, false) => result.OrderBy(key, Comparer<object?>.Default),
                (null, true) => result.OrderByDescending(key, Comparer<object?>.Default),
                (_, false) => ordered.ThenBy(key, Comparer<object?>.Default),
                (_, true) => ordered.ThenByDescending(key, Comparer<object?>.Default),
            };
        }

        result = ordered ?? result;
        if (Skip is { } skip) result = result.Skip(skip);
        if (Top is { } top) result = result.Take(top);
        return true;
    }

    private static bool TryParseCount(string? value, out int? count)
    {
        count = int.TryParse(value, out var n) && n >= 0 ? n : null;
        return count is not null;
    }

    private static bool IsSortable(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        return t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(DateTimeOffset);
    }
}
