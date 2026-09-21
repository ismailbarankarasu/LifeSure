using Microsoft.AspNetCore.Http;

namespace LifeSure.Services.Images;

public interface IImageStorageService
{
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);

    bool Delete(string? imageUrl);
}