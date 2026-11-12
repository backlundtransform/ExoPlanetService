using System.Net;
using ExoplanetHunter.Api.Transit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ExoplanetHunter.Api.Tests;

[TestClass]
public sealed class TransitRefreshServiceTests
{
    private const string Source = "https://example.test/data/transit/";
    private string _root = null!;
    private string _bundled = null!;
    private string _storage = null!;

    [TestInitialize]
    public void Setup()
    {
        _root = Path.Combine(Path.GetTempPath(), "transit-refresh-" + Guid.NewGuid().ToString("N"));
        _bundled = Path.Combine(_root, "bundled");
        _storage = Path.Combine(_root, "storage");
        Directory.CreateDirectory(_bundled);
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, recursive: true);

    private static string Index(params (int Id, string GeneratedAt)[] entries) =>
        "[" + string.Join(",", entries.Select(e => $$"""{"starId":{{e.Id}},"starName":"S{{e.Id}}","generatedAt":"{{e.GeneratedAt}}"}""")) + "]";

    private static string Result(int id, string generatedAt) => $$"""{"starId":{{id}},"generatedAt":"{{generatedAt}}"}""";

    private (TransitRefreshService Service, TransitStore Store, FakeHttp Http, TransitRefreshOptions Options) Create()
    {
        var options = new TransitRefreshOptions { Enabled = true, SourceUrl = Source, StoragePath = _storage };
        var store = TransitStore.Create(_bundled, options, NullLogger.Instance);
        var http = new FakeHttp();
        var service = new TransitRefreshService(store, new HttpClient(http), Options.Create(options), TimeProvider.System,
            NullLogger<TransitRefreshService>.Instance);
        return (service, store, http, options);
    }

    [TestMethod]
    public async Task FirstRefresh_CopiesUnchangedBundledFiles_AndDownloadsChangedOnes()
    {
        File.WriteAllText(Path.Combine(_bundled, "index.json"), Index((1, "2026-11-01T00:00:00Z"), (2, "2026-11-01T00:00:00Z")));
        File.WriteAllText(Path.Combine(_bundled, "1.json"), Result(1, "2026-11-01T00:00:00Z"));
        File.WriteAllText(Path.Combine(_bundled, "2.json"), Result(2, "2026-11-01T00:00:00Z"));
        var (service, store, http, options) = Create();
        http.Files["index.json"] = Index((1, "2026-11-01T00:00:00Z"), (2, "2026-11-08T00:00:00Z"), (3, "2026-11-08T00:00:00Z"));
        http.Files["2.json"] = Result(2, "2026-11-08T00:00:00Z");
        http.Files["3.json"] = Result(3, "2026-11-08T00:00:00Z");

        int downloaded = await service.RefreshAsync(options, CancellationToken.None);

        Assert.AreEqual(2, downloaded);
        CollectionAssert.AreEquivalent(new[] { "index.json", "2.json", "3.json" }, http.Requested);
        Assert.AreEqual(_storage, store.Directory);
        Assert.AreEqual(Result(1, "2026-11-01T00:00:00Z"), File.ReadAllText(store.PathOf("1.json")));
        Assert.AreEqual(Result(2, "2026-11-08T00:00:00Z"), File.ReadAllText(store.PathOf("2.json")));
        Assert.IsTrue(File.Exists(store.PathOf("3.json")));
    }

    [TestMethod]
    public async Task NothingChanged_DownloadsOnlyTheIndex_AndRemovedStarsAreDeleted()
    {
        var (service, store, http, options) = Create();
        http.Files["index.json"] = Index((1, "2026-11-01T00:00:00Z"), (2, "2026-11-01T00:00:00Z"));
        http.Files["1.json"] = Result(1, "2026-11-01T00:00:00Z");
        http.Files["2.json"] = Result(2, "2026-11-01T00:00:00Z");
        await service.RefreshAsync(options, CancellationToken.None);

        http.Requested.Clear();
        http.Files["index.json"] = Index((1, "2026-11-01T00:00:00Z"));
        int downloaded = await service.RefreshAsync(options, CancellationToken.None);

        Assert.AreEqual(0, downloaded);
        CollectionAssert.AreEqual(new[] { "index.json" }, http.Requested);
        Assert.IsFalse(File.Exists(store.PathOf("2.json")));
        Assert.IsTrue(File.Exists(store.PathOf("1.json")));
    }

    [TestMethod]
    public async Task BrokenFile_KeepsServingTheBundledResults()
    {
        File.WriteAllText(Path.Combine(_bundled, "index.json"), Index((1, "2026-11-01T00:00:00Z")));
        var (service, store, http, options) = Create();
        http.Files["index.json"] = Index((1, "2026-11-08T00:00:00Z"));
        http.Files["1.json"] = Result(99, "2026-11-08T00:00:00Z"); // wrong star

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => service.RefreshAsync(options, CancellationToken.None));
        Assert.AreEqual(_bundled, store.Directory);
    }

    [TestMethod]
    public async Task OlderPublishedResults_DoNotReplaceNewerBundledOnes()
    {
        File.WriteAllText(Path.Combine(_bundled, "index.json"), Index((1, "2026-11-08T00:00:00Z")));
        var (service, store, http, options) = Create();
        http.Files["index.json"] = Index((1, "2026-11-01T00:00:00Z"));

        Assert.AreEqual(0, await service.RefreshAsync(options, CancellationToken.None));
        Assert.AreEqual(_bundled, store.Directory);
        CollectionAssert.AreEqual(new[] { "index.json" }, http.Requested);
    }

    [TestMethod]
    public void Startup_PrefersNewerBundledResults()
    {
        Directory.CreateDirectory(_storage);
        File.WriteAllText(Path.Combine(_storage, "index.json"), Index((1, "2026-11-01T00:00:00Z")));
        File.WriteAllText(Path.Combine(_bundled, "index.json"), Index((1, "2026-11-08T00:00:00Z")));
        var options = new TransitRefreshOptions { Enabled = true, StoragePath = _storage };

        Assert.AreEqual(_bundled, TransitStore.Create(_bundled, options, NullLogger.Instance).Directory);

        File.WriteAllText(Path.Combine(_storage, "index.json"), Index((1, "2026-11-15T00:00:00Z")));
        Assert.AreEqual(_storage, TransitStore.Create(_bundled, options, NullLogger.Instance).Directory);
        Assert.AreEqual(_bundled, TransitStore.Create(_bundled, new TransitRefreshOptions { StoragePath = _storage }, NullLogger.Instance).Directory);
    }

    private sealed class FakeHttp : HttpMessageHandler
    {
        public Dictionary<string, string> Files { get; } = [];
        public List<string> Requested { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string name = request.RequestUri!.AbsoluteUri[Source.Length..];
            Requested.Add(name);
            return Task.FromResult(Files.TryGetValue(name, out var body)
                ? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) }
                : new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
