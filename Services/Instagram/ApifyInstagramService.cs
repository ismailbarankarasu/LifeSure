using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LifeSure.Models.Instagram;
using Microsoft.Extensions.Caching.Memory;

namespace LifeSure.Services.Instagram;

public sealed class ApifyInstagramService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache cache, ILogger<ApifyInstagramService> logger) : IInstagramService
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<IReadOnlyList<InstagramPost>> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        var token = configuration["Instagram:ApifyToken"];
        var taskId = configuration["Instagram:TaskId"];

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(taskId))
        {
            logger.LogWarning("Instagram yapılandırması eksik: ApifyToken veya TaskId.");

            return Array.Empty<InstagramPost>();
        }

        var cacheKey = $"instagram:posts:{taskId}";
        var fallbackKey = $"instagram:last-success:{taskId}";

        if (cache.TryGetValue(cacheKey, out IReadOnlyList<InstagramPost>? cached) && cached is not null)
        {
            return cached;
        }

        await _gate.WaitAsync(cancellationToken);

        try
        {
            // Aynı anda gelen isteklerin tekrar API çağrısı yapmasını önler.
            if (cache.TryGetValue(cacheKey, out cached) && cached is not null)
            {
                return cached;
            }

            try
            {
                using var client = httpClientFactory.CreateClient("ApifyInstagram");

                var endpoint =
                    $"actor-tasks/{Uri.EscapeDataString(taskId.Trim())}" +
                    "/runs/last/dataset/items" +
                    "?status=SUCCEEDED&format=json&clean=true&limit=6";

                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());

                using var response = await client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                var items = await response.Content
                    .ReadFromJsonAsync<List<InstagramPost>>(
                        cancellationToken: cancellationToken)
                    ?? new List<InstagramPost>();

                IReadOnlyList<InstagramPost> posts = items
                    .Where(post =>
                        IsInstagramLink(post.Url) &&
                        IsHttpsUrl(post.DisplayUrl))
                    .DistinctBy(post => post.Url)
                    .OrderByDescending(post => post.Timestamp)
                    .Take(6)
                    .ToArray();

                if (posts.Count == 0)
                {
                    throw new JsonException("Sonuçlarda kullanılabilir Instagram görseli yok.");
                }

                cache.Set(cacheKey, posts, TimeSpan.FromMinutes(30));

                cache.Set(fallbackKey, posts, TimeSpan.FromHours(24));

                return posts;
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
                when (exception is HttpRequestException
                      or JsonException
                      or OperationCanceledException)
            {
                logger.LogWarning(
                    exception,
                    "Instagram gönderileri Apify üzerinden okunamadı.");

                var fallback =
                    cache.Get<IReadOnlyList<InstagramPost>>(fallbackKey)
                    ?? Array.Empty<InstagramPost>();

                cache.Set(
                    cacheKey,
                    fallback,
                    TimeSpan.FromMinutes(5));

                return fallback;
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private static bool IsHttpsUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
               && uri.Scheme == Uri.UriSchemeHttps;
    }

    private static bool IsInstagramLink(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
               && uri.Scheme == Uri.UriSchemeHttps
               && (uri.Host.Equals(
                       "instagram.com",
                       StringComparison.OrdinalIgnoreCase)
                   || uri.Host.Equals(
                       "www.instagram.com",
                       StringComparison.OrdinalIgnoreCase));
    }
}