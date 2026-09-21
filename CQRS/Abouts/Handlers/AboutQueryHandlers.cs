using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Abouts.Queries;
using LifeSure.CQRS.Abouts.Results;
using LifeSure.Data;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Abouts.Handlers;

public class GetAboutsQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetAboutsQuery, List<AboutResult>>
{
    public async Task<List<AboutResult>> HandleAsync(
        GetAboutsQuery query,
        CancellationToken cancellationToken = default)
    {
        var aboutsQuery = context.Abouts
            .AsNoTracking()
            .AsQueryable();

        if (query.OnlyActive)
        {
            aboutsQuery = aboutsQuery.Where(x => x.IsActive);
        }

        var abouts = await aboutsQuery
            .Include(x => x.Translations)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return abouts.Select(AboutResult.FromEntity).ToList();
    }
}

public class GetAboutByIdQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetAboutByIdQuery, AboutResult?>
{
    public async Task<AboutResult?> HandleAsync(
        GetAboutByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var about = await context.Abouts
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == query.Id,
                cancellationToken);

        return about is null
            ? null
            : AboutResult.FromEntity(about);
    }
}