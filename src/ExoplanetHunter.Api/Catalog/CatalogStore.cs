using ExoplanetHunter.Api.Contracts;
using ExoplanetHunter.Api.Legacy;
using ExoplanetHunter.Domain.Catalog;

namespace ExoplanetHunter.Api.Catalog;

/// <summary>
/// Holds the catalogue currently being served, together with the precomputed legacy
/// presentation view. Swapping in a new snapshot is atomic: requests in flight keep
/// the view they started with.
/// </summary>
public sealed class CatalogStore
{
    private CatalogView _current;

    public CatalogStore(CatalogSnapshot snapshot) => _current = new CatalogView(new Domain.Catalog.Catalog(snapshot));

    public CatalogView Current => Volatile.Read(ref _current);

    public void Replace(CatalogSnapshot snapshot) => Volatile.Write(ref _current, new CatalogView(new Domain.Catalog.Catalog(snapshot)));
}

/// <summary>A catalogue plus the per-planet presentation records derived from it.</summary>
public sealed class CatalogView
{
    public CatalogView(Domain.Catalog.Catalog catalog)
    {
        Catalog = catalog;
        ExoPlanets = catalog.Planets
            .Select(p =>
            {
                var star = catalog.StarOf(p);
                return LegacyPresentation.ToExoPlanet(p, star, catalog.PlanetsOf(star).Count());
            })
            .ToArray();
    }

    public Domain.Catalog.Catalog Catalog { get; }

    /// <summary>Presentation records for every planet, ordered by planet id.</summary>
    public IReadOnlyList<ExoPlanetsDto> ExoPlanets { get; }

    public int PlanetCount(Star star) => Catalog.PlanetsOf(star).Count();

    public StarDto ToDto(Star star) => StarDto.From(star, PlanetCount(star));
}
