using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Abouts.Queries;
using LifeSure.CQRS.Abouts.Results;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _AboutComponentPartial(
    IQueryHandler<GetAboutsQuery, List<AboutResult>> handler)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var abouts = await handler.HandleAsync(
            new GetAboutsQuery(OnlyActive: true),
            HttpContext.RequestAborted);

        return View(abouts.FirstOrDefault());
    }
}