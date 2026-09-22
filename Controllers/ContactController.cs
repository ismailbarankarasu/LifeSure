using System.ComponentModel.DataAnnotations;
using LifeSure.Mediator.ContactMessages.Commands;
using LifeSure.Mediator.ContactMessages.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Controllers;

[AllowAnonymous]
public class ContactController(
    ISender sender,
    ILogger<ContactController> logger)
    : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("contact-form")]
    public async Task<IActionResult> Send(
        ContactMessageInput model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage)
                    ? "Alanlardan biri geçersiz."
                    : x.ErrorMessage)
                .Distinct()
                .ToArray();

            return BadRequest(new
            {
                success = false,
                message = "Lütfen formdaki bilgileri kontrol ediniz.",
                errors
            });
        }

        try
        {
            await sender.Send(
                new CreateContactMessageCommand(model),
                cancellationToken);

            return Ok(new
            {
                success = true
            });
        }
        catch (ValidationException exception)
        {
            return BadRequest(new
            {
                success = false,
                message = exception.Message
            });
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "İletişim mesajı kaydedilemedi.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message = "Mesaj kaydedilemedi. Lütfen tekrar deneyiniz."
                });
        }
    }
}