using System.ComponentModel.DataAnnotations;
using LifeSure.Mediator.ContactMessages.Commands;
using LifeSure.Mediator.ContactMessages.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace LifeSure.Controllers;

[AllowAnonymous]
public class ContactController(
    ISender sender,
    ILogger<ContactController> logger,
    IStringLocalizer<LifeSure.SharedResource> localizer)
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
                    ? localizer["Validation.Invalid"].Value
                    : x.ErrorMessage)
                .Distinct()
                .ToArray();

            return BadRequest(new
            {
                success = false,
                message = localizer["Contact.ValidationFailed"].Value,
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
        catch (ValidationException)
        {
            return BadRequest(new
            {
                success = false,
                message = localizer["Contact.ValidationFailed"].Value
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
                    message = localizer["Contact.Error"].Value
                });
        }
    }
}