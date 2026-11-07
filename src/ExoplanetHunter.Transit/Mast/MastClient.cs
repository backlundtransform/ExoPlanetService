using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ExoplanetHunter.Transit.Mast;

/// <summary>A TESS light-curve file produced by the SPOC pipeline for one sector.</summary>
public sealed record TessLightCurveFile(int Sector, double ExposureSeconds, string FileName, string DataUri);

/// <summary>
/// Client for the MAST Portal API. MAST is slow (tens of seconds per query) and fails
/// intermittently, so every call is
/// retried with growing delays and downloaded files are cached on disk.
/// </summary>
public sealed class MastClient(HttpClient http, string cacheDirectory)
{
    private static readonly Uri InvokeEndpoint = new("https://mast.stsci.edu/api/v0/invoke");
    private const string DownloadEndpoint = "https://mast.stsci.edu/api/v0.1/Download/file?uri=";
    private static readonly TimeSpan[] RetryDelays = [TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(60)];

    /// <summary>SPOC light curves (product group "LC") with the given exposure time for a TIC id, one per sector.</summary>
    public async Task<IReadOnlyList<TessLightCurveFile>> FindSpocLightCurvesAsync(
        long ticId, double exposureSeconds = 120, CancellationToken cancellationToken = default)
    {
        var observations = await InvokeAsync(new JsonObject
        {
            ["service"] = "Mast.Caom.Filtered",
            ["format"] = "json",
            ["pagesize"] = 500,
            ["page"] = 1,
            ["params"] = new JsonObject
            {
                ["columns"] = "obsid,sequence_number,t_exptime,dataURL",
                ["filters"] = new JsonArray
                {
                    Filter("obs_collection", "TESS"),
                    Filter("dataproduct_type", "timeseries"),
                    Filter("provenance_name", "SPOC"),
                    Filter("target_name", ticId.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                },
            },
        }, cancellationToken);

        // A timeseries observation's dataURL is its light-curve file, so no products call is
        // needed (that call is the slow part of MAST). 2-minute cadence is available for most
        // planet hosts and dense enough for transits.
        return observations
            .Where(o => Math.Abs(o["t_exptime"]!.GetValue<double>() - exposureSeconds) < 1)
            .Select(o => (Sector: o["sequence_number"]!.GetValue<int>(), Uri: (string?)o["dataURL"] ?? ""))
            .Where(o => o.Uri.EndsWith("lc.fits", StringComparison.OrdinalIgnoreCase))
            .Select(o => new TessLightCurveFile(o.Sector, exposureSeconds, o.Uri[(o.Uri.LastIndexOf('/') + 1)..], o.Uri))
            .DistinctBy(f => f.FileName)
            .OrderBy(f => f.Sector)
            .ToArray();
    }

    /// <summary>The file's bytes, from the cache when it was downloaded before.</summary>
    public async Task<byte[]> DownloadAsync(TessLightCurveFile file, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(cacheDirectory);
        string path = Path.Combine(cacheDirectory, file.FileName);
        if (File.Exists(path))
            return await File.ReadAllBytesAsync(path, cancellationToken);

        byte[] bytes = await WithRetriesAsync(async () =>
            await http.GetByteArrayAsync(DownloadEndpoint + Uri.EscapeDataString(file.DataUri), cancellationToken), cancellationToken);

        string temp = path + ".part";
        await File.WriteAllBytesAsync(temp, bytes, cancellationToken);
        File.Move(temp, path, overwrite: true);
        return bytes;
    }

    private async Task<IReadOnlyList<JsonObject>> InvokeAsync(JsonObject request, CancellationToken cancellationToken)
    {
        return await WithRetriesAsync(async () =>
        {
            using var content = new FormUrlEncodedContent([new("request", request.ToJsonString())]);
            using var response = await http.PostAsync(InvokeEndpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<JsonObject>(cancellationToken)
                ?? throw new InvalidDataException("Empty MAST response.");
            string status = (string?)body["status"] ?? "";
            if (status != "COMPLETE")
                throw new HttpRequestException($"MAST returned status '{status}': {(string?)body["msg"]}");

            return (IReadOnlyList<JsonObject>)(body["data"]?.AsArray().OfType<JsonObject>().ToArray() ?? []);
        }, cancellationToken);
    }

    private static async Task<T> WithRetriesAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken)
    {
        for (int attempt = 0; ; attempt++)
        {
            try
            {
                return await action();
            }
            catch (Exception ex) when (attempt < RetryDelays.Length && ex is HttpRequestException or TaskCanceledException or JsonException
                                       && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(RetryDelays[attempt], cancellationToken);
            }
        }
    }

    private static JsonObject Filter(string name, string value) =>
        new() { ["paramName"] = name, ["values"] = new JsonArray(value) };
}
