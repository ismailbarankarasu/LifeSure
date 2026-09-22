using LifeSure.Mediator.SiteSettings.Queries;
using LifeSure.Mediator.SiteSettings.Results;
using MediatR;

namespace LifeSure.Services.SiteSettings;

public class SiteSettingsReader(ISender sender) : ISiteSettingsReader
{
    private Task<SiteSettingResult?>? _settingsTask;

    public Task<SiteSettingResult?> GetAsync(
        CancellationToken cancellationToken = default)
    {
        return _settingsTask ??= sender.Send(
            new GetSiteSettingQuery(),
            cancellationToken);
    }
}