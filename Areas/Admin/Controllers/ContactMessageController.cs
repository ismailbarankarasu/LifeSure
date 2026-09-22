using System.ComponentModel.DataAnnotations;
using LifeSure.Enums;
using LifeSure.Mediator.ContactMessages.Commands;
using LifeSure.Mediator.ContactMessages.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Areas.Admin.Controllers;

public class ContactMessageController(
    ISender sender,
    ILogger<ContactMessageController> logger)
    : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetContactMessagesQuery(page),
            cancellationToken);

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        var message = await sender.Send(
            new GetContactMessageByIdQuery(id),
            cancellationToken);

        return message is null
            ? NotFound()
            : View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        int id,
        ContactMessageStatus? status,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || !status.HasValue)
        {
            TempData["Error"] = "Geçerli bir mesaj durumu seçiniz.";

            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            var updated = await sender.Send(
                new UpdateContactMessageStatusCommand(id, status.Value),
                cancellationToken);

            if (!updated)
            {
                return NotFound();
            }

            TempData["Success"] = "Mesaj durumu güncellendi.";
        }
        catch (ValidationException exception)
        {
            TempData["Error"] = exception.Message;
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "Mesaj durumu güncellenemedi. Id: {ContactMessageId}",
                id);

            TempData["Error"] = "Mesaj durumu güncellenemedi.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}