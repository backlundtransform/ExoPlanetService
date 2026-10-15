using System.Threading.RateLimiting;
using ExoplanetHunter.Api.Catalog;
using ExoplanetHunter.Api.Endpoints;
using ExoplanetHunter.Ingest.Nasa;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// The catalogue shipped with the deployment, or a newer one saved by the scheduled refresh.
var refreshOptions = builder.Configuration.GetSection(CatalogRefreshOptions.Section).Get<CatalogRefreshOptions>() ?? new();
var catalogPath = Path.Combine(AppContext.BaseDirectory, builder.Configuration["Catalog:Path"] ?? "Data/catalog.json.gz");
using (var bootstrap = LoggerFactory.Create(logging => logging.AddConsole()))
{
    var initial = await CatalogRefreshService.LoadInitialAsync(catalogPath, refreshOptions, bootstrap.CreateLogger("Catalog"));
    builder.Services.AddSingleton(new CatalogStore(initial));
}

builder.Services.Configure<CatalogRefreshOptions>(builder.Configuration.GetSection(CatalogRefreshOptions.Section));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpClient<NasaArchiveClient>(http =>
{
    http.Timeout = TimeSpan.FromMinutes(5);
    http.DefaultRequestHeaders.UserAgent.ParseAdd("ExoplanetHunter/1.0 (+https://exoplanethunter.com)");
});
builder.Services.AddHostedService<CatalogRefreshService>();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// The API is public and read-only, so any origin may call it.
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().WithMethods("GET")));

// Anonymous callers are limited per client IP. API keys with their own quotas come later.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue("RateLimiting:PermitsPerMinute", 300),
                Window = TimeSpan.FromMinutes(1),
            }));
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseResponseCompression();
app.UseCors();
app.UseRateLimiter();

app.MapOpenApi();
app.MapHealthChecks("/health");

app.MapCatalogEndpoints();
app.MapExoSolarSystemEndpoints();
app.MapChartEndpoints();
app.MapMiscEndpoints();

var served = app.Services.GetRequiredService<CatalogStore>().Current.Snapshot;
app.Logger.LogInformation("Serving catalog '{Source}' generated {GeneratedAt:u}: {Stars} stars, {Planets} planets",
    served.Source, served.GeneratedAt, served.Stars.Count, served.Planets.Count);

app.Run();

public partial class Program;
