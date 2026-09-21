using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Features.Queries;
using LifeSure.CQRS.Features.Results;
using LifeSure.Data;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Features.Handlers;

public class GetFeaturesQueryHandler(LifeSureDbContext context) : IQueryHandler<GetFeaturesQuery, List<FeatureResult>>
{
    public async Task<List<FeatureResult>> HandleAsync(
        GetFeaturesQuery query,
        CancellationToken cancellationToken = default)
    {
        var featuresQuery = context.Features
            .AsNoTracking()
            .AsQueryable();

        if (query.OnlyActive)
        {
            featuresQuery = featuresQuery.Where(x => x.IsActive);
        }

        var features = await featuresQuery
            .Include(x => x.Translations)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return features.Select(FeatureResult.FromEntity).ToList();
    }
}

public class GetFeatureByIdQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetFeatureByIdQuery, FeatureResult?>
{
    public async Task<FeatureResult?> HandleAsync(
        GetFeatureByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var feature = await context.Features
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == query.Id,
                cancellationToken);

        return feature is null
            ? null
            : FeatureResult.FromEntity(feature);
    }
}