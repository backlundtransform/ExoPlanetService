using System.Threading.RateLimiting;
using ExoplanetHunter.Api.Catalog;
using ExoplanetHunter.Api.Endpoints;
using ExoplanetHunter.Domain.Catalog;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

var catalogPath = Path.Combine(AppContext.BaseDirectory, builder.Configuration["Catalog:Path"] ?? "Data/catalog.json.gz");
var snapshot = await CatalogSnapshot.LoadAsync(catalogPath);
builder.Services.AddSingleton(new CatalogStore(snapshot));

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

app.Logger.LogInformation("Serving catalog '{Source}' generated {GeneratedAt}: {Stars} stars, {Planets} planets",
    snapshot.Source, snapshot.GeneratedAt, snapshot.Stars.Count, snapshot.Planets.Count);

app.Run();

public partial class Program;
