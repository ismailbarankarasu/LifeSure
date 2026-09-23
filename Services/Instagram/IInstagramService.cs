using LifeSure.Models.Instagram;

namespace LifeSure.Services.Instagram;

public interface IInstagramService
{
    Task<IReadOnlyList<InstagramPost>> GetLatestAsync(CancellationToken cancellationToken = default);
}