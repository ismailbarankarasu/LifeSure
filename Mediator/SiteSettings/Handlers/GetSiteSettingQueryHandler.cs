using LifeSure.Data;
using LifeSure.Mediator.SiteSettings.Queries;
using LifeSure.Mediator.SiteSettings.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.SiteSettings.Handlers;

public class GetSiteSettingQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetSiteSettingQuery, SiteSettingResult?>
{
    public async Task<SiteSettingResult?> Handle(
        GetSiteSettingQuery request,
        CancellationToken cancellationToken)
    {
        var setting = await context.SiteSettings
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.SingletonKey == 1,
                cancellationToken);

        return setting is null
            ? null
            : SiteSettingResult.FromEntity(setting);
    }
}