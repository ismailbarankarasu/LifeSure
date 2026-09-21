using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace LifeSure.Services.Images;

public class LocalImageStorageService : IImageStorageService
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

    private static readonly Regex OwnedImagePath = new(
        @"\A/uploads/images/[a-f0-9]{32}\.(jpg|png|webp)\z",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly string _uploadDirectory;
    private readonly ILogger<LocalImageStorageService> _logger;

    public LocalImageStorageService(
        IWebHostEnvironment environment,
        ILogger<LocalImageStorageService> logger)
    {
        _uploadDirectory = Path.Combine(
            environment.WebRootPath,
            "uploads",
            "images");

        _logger = logger;
    }

    public async Task<string> SaveAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length == 0)
        {
            throw new ValidationException(
                "Lütfen bir görsel seçiniz.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ValidationException(
                "Görsel boyutu en fazla 5 MB olabilir.");
        }

        var extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new ValidationException(
                "Yalnızca JPG, JPEG, PNG ve WebP yükleyebilirsiniz.");
        }

        using var content = new MemoryStream();

        await using (var source = file.OpenReadStream())
        {
            var buffer = new byte[81920];
            int bytesRead;

            while ((bytesRead = await source.ReadAsync(
                buffer.AsMemory(),
                cancellationToken)) > 0)
            {
                if (content.Length + bytesRead > MaxFileSize)
                {
                    throw new ValidationException(
                        "Görsel boyutu en fazla 5 MB olabilir.");
                }

                content.Write(buffer, 0, bytesRead);
            }
        }

        if (!HasValidSignature(
            content.GetBuffer().AsSpan(0, (int)content.Length),
            extension))
        {
            throw new ValidationException(
                "Dosya içeriği seçilen görsel türüyle uyuşmuyor.");
        }

        Directory.CreateDirectory(_uploadDirectory);

        // JPEG dosyalarını da .jpg uzantısıyla sakla.
        var savedExtension = extension == ".jpeg"
            ? ".jpg"
            : extension;

        var fileName = $"{Guid.NewGuid():N}{savedExtension}";
        var fullPath = Path.Combine(_uploadDirectory, fileName);

        content.Position = 0;

        var fileCreated = false;

        try
        {
            await using (var destination = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true))
            {
                fileCreated = true;

                await content.CopyToAsync(
                    destination,
                    cancellationToken);
            }
        }
        catch
        {
            if (fileCreated)
            {
                TryDeleteIncompleteFile(fullPath);
            }

            throw;
        }

        return $"/uploads/images/{fileName}";
    }

    public bool Delete(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)
            || !OwnedImagePath.IsMatch(imageUrl))
        {
            return false;
        }

        var fileName = Path.GetFileName(imageUrl);
        var fullPath = Path.Combine(_uploadDirectory, fileName);

        if (!File.Exists(fullPath))
        {
            return false;
        }

        File.Delete(fullPath);

        return true;
    }

    private static bool HasValidSignature(
        ReadOnlySpan<byte> header,
        string extension)
    {
        if (extension is ".jpg" or ".jpeg")
        {
            return header.Length >= 3
                && header[0] == 0xFF
                && header[1] == 0xD8
                && header[2] == 0xFF;
        }

        if (extension == ".png")
        {
            return header.Length >= 8
                && header[0] == 0x89
                && header[1] == 0x50
                && header[2] == 0x4E
                && header[3] == 0x47
                && header[4] == 0x0D
                && header[5] == 0x0A
                && header[6] == 0x1A
                && header[7] == 0x0A;
        }

        if (extension == ".webp")
        {
            return header.Length >= 12
                && header[..4].SequenceEqual("RIFF"u8)
                && header.Slice(8, 4).SequenceEqual("WEBP"u8);
        }

        return false;
    }

    private void TryDeleteIncompleteFile(string fullPath)
    {
        try
        {
            File.Delete(fullPath);
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(
                exception,
                "Yarım kalan görsel dosyası temizlenemedi: {FilePath}",
                fullPath);
        }
    }
}