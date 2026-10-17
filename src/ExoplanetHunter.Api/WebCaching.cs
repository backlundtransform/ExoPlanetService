using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace ExoplanetHunter.Api;

/// <summary>Cache headers for the website's static files.</summary>
public static class WebCaching
{
    public static void Apply(StaticFileResponseContext context)
    {
        var path = context.Context.Request.Path.Value ?? "";
        var headers = context.Context.Response.Headers;

        if (path.StartsWith("/astro3d/assets/", StringComparison.OrdinalIgnoreCase))
        {
            // Vite output: file names contain a content hash.
            headers[HeaderNames.CacheControl] = "public, max-age=31536000, immutable";
        }
        else if (path.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                 || path.EndsWith("sw.js", StringComparison.OrdinalIgnoreCase)
                 || path.EndsWith(".webmanifest", StringComparison.OrdinalIgnoreCase)
                 || path.Equals("/", StringComparison.Ordinal))
        {
            // Must be revalidated so a new deployment (and service worker) is picked up.
            headers[HeaderNames.CacheControl] = "no-cache";
        }
        else
        {
            headers[HeaderNames.CacheControl] = "public, max-age=86400";
        }
    }
}

/// <summary>
/// HTML pages (including the app shell served by the fallback routes) must be revalidated,
/// so browsers pick up a new deployment.
/// </summary>
public sealed class HtmlNoCacheMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var response = context.Response;
            if (response.ContentType?.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) == true
                && !response.Headers.ContainsKey(HeaderNames.CacheControl))
            {
                response.Headers[HeaderNames.CacheControl] = "no-cache";
            }

            return Task.CompletedTask;
        });

        return next(context);
    }
}
