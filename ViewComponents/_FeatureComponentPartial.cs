using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Features.Queries;
using LifeSure.CQRS.Features.Results;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _FeatureComponentPartial(IQueryHandler<GetFeaturesQuery, List<FeatureResult>> handler) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var features = await handler.HandleAsync(
            new GetFeaturesQuery(OnlyActive: true),
            HttpContext.RequestAborted);

        return View(features);
    }
}