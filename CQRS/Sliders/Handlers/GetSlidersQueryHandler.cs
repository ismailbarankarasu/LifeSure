using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Sliders.Queries;
using LifeSure.CQRS.Sliders.Results;
using LifeSure.Data;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Sliders.Handlers;

public class GetSlidersQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetSlidersQuery, List<SliderResult>>
{
    public async Task<List<SliderResult>> HandleAsync(
        GetSlidersQuery query,
        CancellationToken cancellationToken = default)
    {
        var slidersQuery = context.Sliders
            .AsNoTracking()
            .AsQueryable();

        if (query.OnlyActive)
        {
            slidersQuery = slidersQuery.Where(x => x.IsActive);
        }

        var sliders = await slidersQuery
            .Include(x => x.Translations)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return sliders
            .Select(SliderResult.FromEntity)
            .ToList();
    }
}