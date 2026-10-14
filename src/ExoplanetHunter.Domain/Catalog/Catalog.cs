namespace ExoplanetHunter.Domain.Catalog;

/// <summary>
/// Immutable, indexed view of a <see cref="CatalogSnapshot"/>. Built once per snapshot and
/// shared by all requests; stars, planets and constellations are ordered by id.
/// </summary>
public sealed class Catalog
{
    private readonly Dictionary<int, Star> _starsById;
    private readonly Dictionary<int, Planet> _planetsById;
    private readonly Dictionary<int, Constellation> _constellationsById;
    private readonly Dictionary<string, Star> _starsByName;
    private readonly ILookup<int, Planet> _planetsByStar;
    private readonly ILookup<string, Star> _starsByConstellation;

    public Catalog(CatalogSnapshot snapshot)
    {
        Source = snapshot.Source;
        GeneratedAt = snapshot.GeneratedAt;
        Stars = snapshot.Stars.OrderBy(s => s.Id).ToArray();
        Planets = snapshot.Planets.OrderBy(p => p.Id).ToArray();
        Constellations = snapshot.Constellations.OrderBy(c => c.Id).ToArray();

        _starsById = Stars.ToDictionary(s => s.Id);
        _planetsById = Planets.ToDictionary(p => p.Id);
        _constellationsById = Constellations.ToDictionary(c => c.Id);
        _starsByName = Stars.ToDictionary(s => s.Name, StringComparer.Ordinal);
        _planetsByStar = Planets.ToLookup(p => p.StarId);
        _starsByConstellation = Stars.Where(s => s.Constellation is not null).ToLookup(s => s.Constellation!, StringComparer.Ordinal);

        var orphan = Planets.FirstOrDefault(p => !_starsById.ContainsKey(p.StarId));
        if (orphan is not null)
            throw new InvalidDataException($"Planet {orphan.Id} ({orphan.Name}) refers to unknown star {orphan.StarId}.");
    }

    public string Source { get; }
    public DateTimeOffset GeneratedAt { get; }
    public IReadOnlyList<Star> Stars { get; }
    public IReadOnlyList<Planet> Planets { get; }
    public IReadOnlyList<Constellation> Constellations { get; }

    public Star? FindStar(int id) => _starsById.GetValueOrDefault(id);
    public Star? FindStar(string name) => _starsByName.GetValueOrDefault(name);
    public Planet? FindPlanet(int id) => _planetsById.GetValueOrDefault(id);
    public Constellation? FindConstellation(int id) => _constellationsById.GetValueOrDefault(id);

    public Star StarOf(Planet planet) => _starsById[planet.StarId];
    public IEnumerable<Planet> PlanetsOf(Star star) => _planetsByStar[star.Id];
    public IEnumerable<Star> StarsIn(Constellation constellation) => _starsByConstellation[constellation.Name];
}
