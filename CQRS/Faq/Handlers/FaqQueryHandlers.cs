using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Faqs.Queries;
using LifeSure.CQRS.Faqs.Results;
using LifeSure.Data;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Faqs.Handlers;

public class GetFaqsQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetFaqsQuery, List<FaqResult>>
{
    public async Task<List<FaqResult>> HandleAsync(
        GetFaqsQuery query,
        CancellationToken cancellationToken = default)
    {
        var faqsQuery = context.Faqs
            .AsNoTracking()
            .AsQueryable();

        if (query.OnlyActive)
        {
            faqsQuery = faqsQuery.Where(x => x.IsActive);
        }

        var faqs = await faqsQuery
            .Include(x => x.Translations)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return faqs
            .Select(FaqResult.FromEntity)
            .ToList();
    }
}

public class GetFaqByIdQueryHandler(LifeSureDbContext context)
    : IQueryHandler<GetFaqByIdQuery, FaqResult?>
{
    public async Task<FaqResult?> HandleAsync(
        GetFaqByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var faq = await context.Faqs
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == query.Id,
                cancellationToken);

        return faq is null
            ? null
            : FaqResult.FromEntity(faq);
    }
}