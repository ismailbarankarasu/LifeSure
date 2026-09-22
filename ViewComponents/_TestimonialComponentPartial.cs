using LifeSure.Mediator.Testimonials.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _TestimonialComponentPartial(ISender sender)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var testimonials = await sender.Send(
            new GetTestimonialsQuery(OnlyActive: true),
            HttpContext.RequestAborted);

        return View(testimonials);
    }
}