using System.ComponentModel.DataAnnotations;
using LifeSure.Areas.Admin.Models;
using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Abouts.Commands;
using LifeSure.CQRS.Abouts.Models;
using LifeSure.CQRS.Abouts.Queries;
using LifeSure.CQRS.Abouts.Results;
using LifeSure.Services.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Areas.Admin.Controllers;

public class AboutController(
    IQueryHandler<GetAboutsQuery, List<AboutResult>> listHandler,
    IQueryHandler<GetAboutByIdQuery, AboutResult?> detailHandler,
    ICommandHandler<SaveAboutCommand, int?> saveHandler,
    ICommandHandler<DeleteAboutCommand, bool> deleteHandler,
    IImageStorageService imageStorage,
    ILogger<AboutController> logger) : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var abouts = await listHandler.HandleAsync(
            new GetAboutsQuery(),
            cancellationToken);

        return View(abouts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return FormView(new AboutFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var about = await detailHandler.HandleAsync(
            new GetAboutByIdQuery(id),
            cancellationToken);

        if (about is null)
        {
            return NotFound();
        }

        var model = new AboutFormViewModel
        {
            Id = about.Id,
            ExistingImageUrl = about.ImageUrl,
            IsActive = about.IsActive,
            DisplayOrder = about.DisplayOrder,

            Translations = new[] { "tr", "en" }
                .Select(language =>
                {
                    var translation = about.Translations
                        .FirstOrDefault(x => x.LanguageCode == language);

                    return new AboutTranslationFormModel
                    {
                        LanguageCode = language,
                        Subtitle = translation?.Subtitle,
                        Title = translation?.Title,
                        Description = translation?.Description
                    };
                })
                .ToList()
        };

        return FormView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int? id,
        AboutFormViewModel model,
        CancellationToken cancellationToken)
    {
        AboutResult? current = null;

        if (id.HasValue)
        {
            current = await detailHandler.HandleAsync(
                new GetAboutByIdQuery(id.Value),
                cancellationToken);

            if (current is null)
            {
                return NotFound();
            }

            model.Id = current.Id;
            model.ExistingImageUrl = current.ImageUrl;
        }

        if (!id.HasValue
            && (model.ImageFile is null || model.ImageFile.Length == 0))
        {
            ModelState.AddModelError(
                nameof(model.ImageFile),
                "Lütfen bir görsel seçiniz.");
        }

        if (!ModelState.IsValid)
        {
            return FormView(model);
        }

        string? uploadedImageUrl = null;
        var saved = false;

        try
        {
            var imageUrl = current?.ImageUrl ?? string.Empty;

            if (model.ImageFile is not null)
            {
                uploadedImageUrl = await imageStorage.SaveAsync(
                    model.ImageFile,
                    cancellationToken);

                imageUrl = uploadedImageUrl;
            }

            var input = new AboutInput
            {
                ImageUrl = imageUrl,
                IsActive = model.IsActive,
                DisplayOrder = model.DisplayOrder,

                Translations = model.Translations
                    .Where(x => x.LanguageCode == "tr" || x.HasContent())
                    .Select(x => new AboutTranslationDto
                    {
                        LanguageCode = x.LanguageCode,
                        Subtitle = x.Subtitle ?? string.Empty,
                        Title = x.Title ?? string.Empty,
                        Description = x.Description ?? string.Empty
                    })
                    .ToList()
            };

            var savedId = await saveHandler.HandleAsync(
                new SaveAboutCommand(id, input),
                cancellationToken);

            if (!savedId.HasValue)
            {
                return NotFound();
            }

            saved = true;
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (Exception exception)
            when (exception is DbUpdateException
                or IOException
                or UnauthorizedAccessException)
        {
            logger.LogError(
                exception,
                "Hakkımızda kaydedilemedi. AboutId: {AboutId}",
                id);

            ModelState.AddModelError(
                string.Empty,
                "Kayıt sırasında bir hata oluştu.");
        }
        finally
        {
            if (!saved)
            {
                TryDeleteImage(uploadedImageUrl);
            }
        }

        if (!saved)
        {
            return FormView(model);
        }

        if (uploadedImageUrl is not null && current is not null)
        {
            TryDeleteImage(current.ImageUrl);
        }

        TempData["SuccessMessage"] =
            "Hakkımızda içeriği başarıyla kaydedildi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var about = await detailHandler.HandleAsync(
            new GetAboutByIdQuery(id),
            cancellationToken);

        if (about is null)
        {
            return NotFound();
        }

        try
        {
            var deleted = await deleteHandler.HandleAsync(
                new DeleteAboutCommand(id),
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
                "Hakkımızda silinemedi. AboutId: {AboutId}",
                id);

            TempData["ErrorMessage"] = "Kayıt silinemedi.";

            return RedirectToAction(nameof(Index));
        }

        TryDeleteImage(about.ImageUrl);

        TempData["SuccessMessage"] = "Kayıt başarıyla silindi.";

        return RedirectToAction(nameof(Index));
    }

    private ViewResult FormView(AboutFormViewModel model)
    {
        model.Translations ??=
        [
            new() { LanguageCode = "tr" },
            new() { LanguageCode = "en" }
        ];

        ViewData["Title"] = model.Id == 0
            ? "Hakkımızda Ekle"
            : "Hakkımızda Düzenle";

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
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(
                exception,
                "Görsel temizlenemedi: {ImageUrl}",
                imageUrl);
        }
    }
}