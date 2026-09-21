using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Sliders.Queries;
using LifeSure.CQRS.Sliders.Results;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents
{
    public class _SliderComponentPartial(
        IQueryHandler<GetSlidersQuery, List<SliderResult>> handler) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sliders = await handler.HandleAsync(
                new GetSlidersQuery(OnlyActive: true),
                HttpContext.RequestAborted);
            return View(sliders);
        }
    }
}
