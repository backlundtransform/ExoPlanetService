namespace ExoplanetHunter.Domain.Catalog;

/// <summary>An IAU constellation, identified by its three-letter abbreviation.</summary>
public sealed record Constellation
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}
