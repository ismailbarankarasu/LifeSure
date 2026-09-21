using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Faqs.Queries;
using LifeSure.CQRS.Faqs.Results;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _FaqComponentPartial(
    IQueryHandler<GetFaqsQuery, List<FaqResult>> handler)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var faqs = await handler.HandleAsync(
            new GetFaqsQuery(OnlyActive: true),
            HttpContext.RequestAborted);

        return View(faqs);
    }
}