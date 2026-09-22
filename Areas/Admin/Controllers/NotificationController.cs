using LifeSure.Mediator.Notifications.Commands;
using LifeSure.Mediator.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.Areas.Admin.Controllers;

public class NotificationController(ISender sender)
    : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(
        int page = 1,
        bool onlyUnread = false,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetNotificationsQuery(page, onlyUnread),
            cancellationToken);

        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(
        int id,
        CancellationToken cancellationToken)
    {
        var messageId = await sender.Send(
            new OpenNotificationCommand(id),
            cancellationToken);

        if (!messageId.HasValue)
        {
            return NotFound();
        }

        return RedirectToAction(
            "Details",
            "ContactMessage",
            new
            {
                area = "Admin",
                id = messageId.Value
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllAsRead(
        bool onlyUnread = false,
        CancellationToken cancellationToken = default)
    {
        var updatedCount = await sender.Send(
            new MarkAllNotificationsAsReadCommand(),
            cancellationToken);

        TempData["Success"] = updatedCount > 0
            ? $"{updatedCount} bildirim okundu olarak işaretlendi."
            : "Okunmamış bildirim bulunmuyor.";

        return RedirectToAction(
            nameof(Index),
            new { onlyUnread });
    }
}