using LifeSure.Mediator.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _ServiceComponentPartial(ISender sender)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var services = await sender.Send(
            new GetServicesQuery(OnlyActive: true),
            HttpContext.RequestAborted);

        return View(services);
    }
}