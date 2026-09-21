using System.ComponentModel.DataAnnotations;
using LifeSure.Areas.Admin.Models;
using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Sliders.Commands;
using LifeSure.CQRS.Sliders.Models;
using LifeSure.CQRS.Sliders.Queries;
using LifeSure.CQRS.Sliders.Results;
using LifeSure.Services.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Areas.Admin.Controllers;

public class SliderController(
    IQueryHandler<GetSlidersQuery, List<SliderResult>> listHandler,
    IQueryHandler<GetSliderByIdQuery, SliderResult?> detailHandler,
    ICommandHandler<CreateSliderCommand, int> createHandler,
    ICommandHandler<UpdateSliderCommand, bool> updateHandler,
    ICommandHandler<DeleteSliderCommand, bool> deleteHandler,
    IImageStorageService imageStorage,
    ILogger<SliderController> logger) : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var sliders = await listHandler.HandleAsync(
            new GetSlidersQuery(),
            cancellationToken);

        return View(sliders);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return FormView(new SliderFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        SliderFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.ImageFile is null || model.ImageFile.Length == 0)
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
            uploadedImageUrl = await imageStorage.SaveAsync(
                model.ImageFile!,
                cancellationToken);

            var input = ToInput(model, uploadedImageUrl);

            await createHandler.HandleAsync(
                new CreateSliderCommand(input),
                cancellationToken);

            saved = true;
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);
        }
        catch (Exception exception)
            when (exception is DbUpdateException
                or IOException
                or UnauthorizedAccessException)
        {
            logger.LogError(exception, "Slider oluşturulamadı.");

            ModelState.AddModelError(
                string.Empty,
                "Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyiniz.");
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

        TempData["SuccessMessage"] = "Slider başarıyla eklendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var slider = await detailHandler.HandleAsync(
            new GetSliderByIdQuery(id),
            cancellationToken);

        if (slider is null)
        {
            return NotFound();
        }

        var model = new SliderFormViewModel
        {
            Id = slider.Id,
            ExistingImageUrl = slider.ImageUrl,
            VideoId = slider.VideoId,
            ButtonUrl = slider.ButtonUrl,
            DisplayOrder = slider.DisplayOrder,
            IsActive = slider.IsActive,
            Translations = new[] { "tr", "en" }
                .Select(language =>
                {
                    var translation = slider.Translations
                        .FirstOrDefault(x => x.LanguageCode == language);

                    return new SliderTranslationFormModel
                    {
                        LanguageCode = language,
                        Subtitle = translation?.Subtitle,
                        Title = translation?.Title,
                        Description = translation?.Description,
                        ButtonText = translation?.ButtonText
                    };
                })
                .ToList()
        };

        return FormView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        SliderFormViewModel model,
        CancellationToken cancellationToken)
    {
        var currentSlider = await detailHandler.HandleAsync(
            new GetSliderByIdQuery(id),
            cancellationToken);

        if (currentSlider is null)
        {
            return NotFound();
        }

        // Bu bilgiler formdan değil, mevcut kayıttan alınır.
        model.Id = currentSlider.Id;
        model.ExistingImageUrl = currentSlider.ImageUrl;

        if (!ModelState.IsValid)
        {
            return FormView(model);
        }

        string? uploadedImageUrl = null;
        var saved = false;

        try
        {
            var imageUrl = currentSlider.ImageUrl;

            if (model.ImageFile is not null)
            {
                uploadedImageUrl = await imageStorage.SaveAsync(
                    model.ImageFile,
                    cancellationToken);

                imageUrl = uploadedImageUrl;
            }

            var input = ToInput(model, imageUrl);

            saved = await updateHandler.HandleAsync(
                new UpdateSliderCommand(id, input),
                cancellationToken);

            if (!saved)
            {
                return NotFound();
            }
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);
        }
        catch (Exception exception)
            when (exception is DbUpdateException
                or IOException
                or UnauthorizedAccessException)
        {
            logger.LogError(
                exception,
                "Slider güncellenemedi. SliderId: {SliderId}",
                id);

            ModelState.AddModelError(
                string.Empty,
                "Güncelleme sırasında bir hata oluştu. Lütfen tekrar deneyiniz.");
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

        // Eski görsel yalnızca başarılı kayıt sonrasında temizlenir.
        if (uploadedImageUrl is not null)
        {
            TryDeleteImage(currentSlider.ImageUrl);
        }

        TempData["SuccessMessage"] = "Slider başarıyla güncellendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var slider = await detailHandler.HandleAsync(
            new GetSliderByIdQuery(id),
            cancellationToken);

        if (slider is null)
        {
            return NotFound();
        }

        try
        {
            var deleted = await deleteHandler.HandleAsync(
                new DeleteSliderCommand(id),
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
                "Slider silinemedi. SliderId: {SliderId}",
                id);

            TempData["ErrorMessage"] = "Slider silinemedi.";

            return RedirectToAction(nameof(Index));
        }

        TryDeleteImage(slider.ImageUrl);

        TempData["SuccessMessage"] = "Slider başarıyla silindi.";

        return RedirectToAction(nameof(Index));
    }

    private ViewResult FormView(SliderFormViewModel model)
    {
        model.Translations ??=
        [
            new() { LanguageCode = "tr" },
            new() { LanguageCode = "en" }
        ];

        ViewData["Title"] = model.Id == 0
            ? "Slider Ekle"
            : "Slider Düzenle";

        return View("Form", model);
    }

    private static SliderInput ToInput(
        SliderFormViewModel model,
        string imageUrl)
    {
        return new SliderInput
        {
            ImageUrl = imageUrl,
            VideoId = string.IsNullOrEmpty(model.VideoId)
                ? null
                : model.VideoId,
            ButtonUrl = model.ButtonUrl,
            DisplayOrder = model.DisplayOrder,
            IsActive = model.IsActive,

            Translations = model.Translations
                .Where(x => x.LanguageCode == "tr" || x.HasContent())
                .Select(x => new SliderTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Subtitle = x.Subtitle ?? string.Empty,
                    Title = x.Title ?? string.Empty,
                    Description = x.Description ?? string.Empty,
                    ButtonText = x.ButtonText ?? string.Empty
                })
                .ToList()
        };
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