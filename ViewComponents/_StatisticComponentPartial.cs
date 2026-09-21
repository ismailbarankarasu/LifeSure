using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Statistics.Queries;
using LifeSure.CQRS.Statistics.Results;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _StatisticComponentPartial(
    IQueryHandler<GetStatisticsQuery, List<StatisticResult>> handler)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var statistics = await handler.HandleAsync(
            new GetStatisticsQuery(OnlyActive: true),
            HttpContext.RequestAborted);

        return View(statistics);
    }
}