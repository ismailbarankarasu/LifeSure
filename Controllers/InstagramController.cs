using System.Security.Cryptography;
using System.Text;
using LifeSure.Services.Instagram;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LifeSure.Controllers;

[AllowAnonymous]
[Route("instagram")]
public sealed class InstagramController(
    IInstagramService instagramService,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    ILogger<InstagramController> logger) : Controller
{
    private static readonly SemaphoreSlim ImageLock = new(1, 1);

    private sealed record CachedImage(byte[] Bytes, string ContentType);

    [HttpGet("image/{id}")]
    public async Task<IActionResult> Image(
        string id,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) ||
            id.Length > 40 ||
            !id.All(char.IsAsciiDigit))
        {
            return BadRequest();
        }

        var posts = await instagramService.GetLatestAsync(
            cancellationToken);

        var post = posts.FirstOrDefault(x => x.Id == id);

        if (post is null ||
            !Uri.TryCreate(post.DisplayUrl, UriKind.Absolute, out var imageUri) ||
            !IsAllowedImageUri(imageUri))
        {
            return NotFound();
        }

        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(post.DisplayUrl)));

        var cacheKey = $"instagram:image:{hash}";

        if (cache.TryGetValue<CachedImage>(cacheKey, out var cached) &&
            cached is not null)
        {
            return CreateImageResult(cached);
        }

        await ImageLock.WaitAsync(cancellationToken);

        try
        {
            if (cache.TryGetValue<CachedImage>(cacheKey, out cached) &&
                cached is not null)
            {
                return CreateImageResult(cached);
            }

            try
            {
                using var client = httpClientFactory.CreateClient("InstagramImages");

                using var response = await client.GetAsync(
                    imageUri,
                    HttpCompletionOption.ResponseContentRead,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var contentType = response.Content.Headers
                    .ContentType?.MediaType?.ToLowerInvariant();

                if (contentType is not
                    ("image/jpeg" or "image/png" or "image/webp"))
                {
                    throw new HttpRequestException(
                        "Beklenen görsel formatı alınamadı.");
                }

                var bytes = await response.Content.ReadAsByteArrayAsync(
                    cancellationToken);

                if (bytes.Length == 0)
                {
                    throw new HttpRequestException("Görsel yanıtı boş.");
                }

                var image = new CachedImage(bytes, contentType);

                cache.Set(
                    cacheKey,
                    image,
                    TimeSpan.FromMinutes(30));

                return CreateImageResult(image);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
                when (exception is HttpRequestException
                      or OperationCanceledException)
            {
                logger.LogWarning(
                    "Instagram görseli indirilemedi. " +
                    "PostId: {PostId}, Hata: {ErrorType}",
                    id,
                    exception.GetType().Name);

                cache.Set(
                    cacheKey,
                    new CachedImage(Array.Empty<byte>(), ""),
                    TimeSpan.FromMinutes(1));

                return StatusCode(StatusCodes.Status502BadGateway);
            }
        }
        finally
        {
            ImageLock.Release();
        }
    }

    private IActionResult CreateImageResult(CachedImage image)
    {
        if (image.Bytes.Length == 0)
        {
            return StatusCode(StatusCodes.Status502BadGateway);
        }

        Response.Headers["Cache-Control"] = "public,max-age=1800";
        Response.Headers["X-Content-Type-Options"] = "nosniff";

        return File(image.Bytes, image.ContentType);
    }

    private static bool IsAllowedImageUri(Uri uri)
    {
        return uri.Scheme == Uri.UriSchemeHttps
               && uri.IsDefaultPort
               && string.IsNullOrEmpty(uri.UserInfo)
               && (
                   uri.Host.EndsWith(
                       ".cdninstagram.com",
                       StringComparison.OrdinalIgnoreCase)
                   || uri.Host.EndsWith(
                       ".fbcdn.net",
                       StringComparison.OrdinalIgnoreCase)
               );
    }
}