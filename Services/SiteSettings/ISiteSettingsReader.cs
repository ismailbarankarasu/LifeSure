using LifeSure.Mediator.SiteSettings.Results;

namespace LifeSure.Services.SiteSettings;

public interface ISiteSettingsReader
{
    Task<SiteSettingResult?> GetAsync(
        CancellationToken cancellationToken = default);
}