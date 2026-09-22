using LifeSure.Mediator.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _AdminNotificationComponentPartial(ISender sender)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!HttpContext.User.IsInRole("Admin"))
        {
            return Content(string.Empty);
        }

        var unreadCount = await sender.Send(
            new GetUnreadNotificationCountQuery(),
            HttpContext.RequestAborted);

        return View(unreadCount);
    }
}