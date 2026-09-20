using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Sliders.Queries;
using LifeSure.CQRS.Sliders.Results;
using LifeSure.Data;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Sliders.Handlers;

public class GetSliderByIdQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetSliderByIdQuery, SliderResult?>
{
    public async Task<SliderResult?> HandleAsync(
        GetSliderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var slider = await context.Sliders
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == query.Id,
                cancellationToken);

        return slider is null
            ? null
            : SliderResult.FromEntity(slider);
    }
}