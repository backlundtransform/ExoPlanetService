using System.IO.Compression;
using System.Text.Json;

namespace ExoplanetHunter.Domain.Catalog;

/// <summary>
/// The serialized form of a catalogue: what the ingest writes and the API loads.
/// Stored as JSON, optionally gzip-compressed.
/// </summary>
public sealed record CatalogSnapshot
{
    public required string Source { get; init; }
    public required DateTimeOffset GeneratedAt { get; init; }
    public required IReadOnlyList<Constellation> Constellations { get; init; }
    public required IReadOnlyList<Star> Stars { get; init; }
    public required IReadOnlyList<Planet> Planets { get; init; }

    /// <summary>
    /// Next id to assign to a new star or planet. Ids of objects that left the catalogue are
    /// never reused, so this can be higher than the largest id present.
    /// </summary>
    public int? NextStarId { get; init; }

    public int? NextPlanetId { get; init; }

    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<CatalogSnapshot> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var file = File.OpenRead(path);
        return await LoadAsync(file, cancellationToken);
    }

    public static async Task<CatalogSnapshot> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (!stream.CanSeek)
        {
            var copy = new MemoryStream();
            await stream.CopyToAsync(copy, cancellationToken);
            copy.Position = 0;
            stream = copy;
        }

        Stream content = IsGzip(stream) ? new GZipStream(stream, CompressionMode.Decompress) : stream;
        return await JsonSerializer.DeserializeAsync<CatalogSnapshot>(content, JsonOptions, cancellationToken)
            ?? throw new InvalidDataException("Catalog snapshot is empty.");
    }

    public async Task SaveAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var file = File.Create(path);
        Stream content = path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
            ? new GZipStream(file, CompressionLevel.Optimal)
            : file;
        await using (content)
        {
            await JsonSerializer.SerializeAsync(content, this, JsonOptions, cancellationToken);
        }
    }

    private static bool IsGzip(Stream stream)
    {
        Span<byte> header = stackalloc byte[2];
        long start = stream.Position;
        int read = stream.ReadAtLeast(header, 2, throwOnEndOfStream: false);
        stream.Position = start;
        return read == 2 && header[0] == 0x1f && header[1] == 0x8b;
    }
}
