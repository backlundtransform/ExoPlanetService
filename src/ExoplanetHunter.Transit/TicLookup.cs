using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace ExoplanetHunter.Transit;

/// <summary>Looks up TESS Input Catalog ids of host stars in the NASA Exoplanet Archive.</summary>
public sealed class TicLookup(HttpClient http)
{
    private static readonly Uri TapEndpoint = new("https://exoplanetarchive.ipac.caltech.edu/TAP/sync");

    public async Task<IReadOnlyDictionary<string, long>> FindAsync(IEnumerable<string> hostNames, CancellationToken cancellationToken = default)
    {
        var names = hostNames.Distinct().ToArray();
        if (names.Length == 0)
            return new Dictionary<string, long>();

        string list = string.Join(",", names.Select(n => $"'{n.Replace("'", "''")}'"));
        string query = $"select distinct hostname, tic_id from pscomppars where tic_id is not null and hostname in ({list})";
        var rows = await http.GetFromJsonAsync<JsonArray>(
            new Uri(TapEndpoint, $"?query={Uri.EscapeDataString(query)}&format=json"), cancellationToken) ?? [];

        var result = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var row in rows.OfType<JsonObject>())
        {
            // tic_id is published as "TIC 100100827".
            string? tic = (string?)row["tic_id"];
            if ((string?)row["hostname"] is { } host && tic is not null
                && long.TryParse(tic.Replace("TIC", "").Trim(), out long id))
                result[host] = id;
        }

        return result;
    }
}
