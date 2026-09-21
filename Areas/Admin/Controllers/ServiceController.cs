using System.ComponentModel.DataAnnotations;
using LifeSure.Areas.Admin.Models;
using LifeSure.Mediator.Services.Commands;
using LifeSure.Mediator.Services.Models;
using LifeSure.Mediator.Services.Queries;
using LifeSure.Services.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Areas.Admin.Controllers;

public class ServiceController(
    ISender sender,
    IImageStorageService imageStorage,
    ILogger<ServiceController> logger)
    : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var services = await sender.Send(
            new GetServicesQuery(),
            cancellationToken);

        return View(services);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return FormView(new ServiceFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var service = await sender.Send(
            new GetServiceByIdQuery(id),
            cancellationToken);

        if (service is null)
        {
            return NotFound();
        }

        var turkish = service.Translations
            .FirstOrDefault(x => x.LanguageCode == "tr");

        var english = service.Translations
            .FirstOrDefault(x => x.LanguageCode == "en");

        var model = new ServiceFormViewModel
        {
            Id = service.Id,
            ExistingImageUrl = service.ImageUrl,
            IconClass = service.IconClass,
            DisplayOrder = service.DisplayOrder,
            IsActive = service.IsActive,
            TurkishTitle = turkish?.Title ?? string.Empty,
            TurkishShortDescription =
                turkish?.ShortDescription ?? string.Empty,
            TurkishDescription = turkish?.Description ?? string.Empty,
            EnglishTitle = english?.Title,
            EnglishShortDescription = english?.ShortDescription,
            EnglishDescription = english?.Description
        };

        return FormView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int? id,
        ServiceFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;

        if (id.HasValue)
        {
            var existing = await sender.Send(
                new GetServiceByIdQuery(id.Value),
                cancellationToken);

            if (existing is null)
            {
                return NotFound();
            }

            model.ExistingImageUrl = existing.ImageUrl;
        }

        if (!id.HasValue
            && (model.ImageFile is null || model.ImageFile.Length == 0))
        {
            ModelState.AddModelError(
                nameof(model.ImageFile),
                "Hizmet görseli seçiniz.");
        }

        if (!ModelState.IsValid)
        {
            return FormView(model);
        }

        string? uploadedImageUrl = null;
        var saved = false;

        try
        {
            if (model.ImageFile is { Length: > 0 })
            {
                uploadedImageUrl = await imageStorage.SaveAsync(
                    model.ImageFile,
                    cancellationToken);
            }

            var input = new ServiceInput
            {
                ImageUrl = uploadedImageUrl
                    ?? model.ExistingImageUrl
                    ?? string.Empty,
                IconClass = model.IconClass.Trim(),
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                Translations =
                [
                    new ServiceTranslationDto
                    {
                        LanguageCode = "tr",
                        Title = model.TurkishTitle.Trim(),
                        ShortDescription =
                            model.TurkishShortDescription.Trim(),
                        Description = model.TurkishDescription.Trim()
                    }
                ]
            };

            if (model.HasEnglishContent())
            {
                input.Translations.Add(new ServiceTranslationDto
                {
                    LanguageCode = "en",
                    Title = model.EnglishTitle!.Trim(),
                    ShortDescription =
                        model.EnglishShortDescription!.Trim(),
                    Description = model.EnglishDescription!.Trim()
                });
            }

            if (id.HasValue)
            {
                var updated = await sender.Send(
                    new UpdateServiceCommand(id.Value, input),
                    cancellationToken);

                if (!updated)
                {
                    return NotFound();
                }
            }
            else
            {
                await sender.Send(
                    new CreateServiceCommand(input),
                    cancellationToken);
            }

            saved = true;
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);
        }
        catch (Exception exception) when (
            exception is DbUpdateException
            or IOException
            or UnauthorizedAccessException)
        {
            logger.LogError(
                exception,
                "Hizmet kaydedilemedi. Id: {ServiceId}",
                id);

            ModelState.AddModelError(
                string.Empty,
                "Hizmet kaydedilemedi. Lütfen tekrar deneyiniz.");
        }
        finally
        {
            if (!saved && uploadedImageUrl is not null)
            {
                TryDeleteImage(uploadedImageUrl);
            }
        }

        if (!saved)
        {
            return FormView(model);
        }

        if (uploadedImageUrl is not null)
        {
            TryDeleteImage(model.ExistingImageUrl);
        }

        TempData["Success"] = id.HasValue
            ? "Hizmet güncellendi."
            : "Hizmet oluşturuldu.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var service = await sender.Send(
            new GetServiceByIdQuery(id),
            cancellationToken);

        if (service is null)
        {
            return NotFound();
        }

        try
        {
            var deleted = await sender.Send(
                new DeleteServiceCommand(id),
                cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "Hizmet silinemedi. Id: {ServiceId}",
                id);

            TempData["Error"] =
                "Hizmet silinemedi. Lütfen tekrar deneyiniz.";

            return RedirectToAction(nameof(Index));
        }

        TryDeleteImage(service.ImageUrl);

        TempData["Success"] = "Hizmet silindi.";

        return RedirectToAction(nameof(Index));
    }

    private ViewResult FormView(ServiceFormViewModel model)
    {
        ViewData["Title"] = model.Id.HasValue
            ? "Hizmet Düzenle"
            : "Yeni Hizmet";

        return View("Form", model);
    }

    private void TryDeleteImage(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        try
        {
            imageStorage.Delete(imageUrl);
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(
                exception,
                "Hizmet görseli temizlenemedi: {ImageUrl}",
                imageUrl);
        }
    }
}