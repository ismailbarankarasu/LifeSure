using System.ComponentModel.DataAnnotations;
using LifeSure.Areas.Admin.Models;
using LifeSure.Mediator.Testimonials.Commands;
using LifeSure.Mediator.Testimonials.Models;
using LifeSure.Mediator.Testimonials.Queries;
using LifeSure.Services.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Areas.Admin.Controllers;

public class TestimonialController(
    ISender sender,
    IImageStorageService imageStorage,
    ILogger<TestimonialController> logger)
    : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var testimonials = await sender.Send(
            new GetTestimonialsQuery(),
            cancellationToken);

        return View(testimonials);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return FormView(new TestimonialFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var testimonial = await sender.Send(
            new GetTestimonialByIdQuery(id),
            cancellationToken);

        if (testimonial is null)
        {
            return NotFound();
        }

        var turkish = testimonial.Translations
            .FirstOrDefault(x => x.LanguageCode == "tr");

        var english = testimonial.Translations
            .FirstOrDefault(x => x.LanguageCode == "en");

        var model = new TestimonialFormViewModel
        {
            Id = testimonial.Id,
            FullName = testimonial.FullName,
            ExistingImageUrl = testimonial.ImageUrl,
            Rating = testimonial.Rating,
            DisplayOrder = testimonial.DisplayOrder,
            IsActive = testimonial.IsActive,
            TurkishTitle = turkish?.Title ?? string.Empty,
            TurkishComment = turkish?.Comment ?? string.Empty,
            EnglishTitle = english?.Title,
            EnglishComment = english?.Comment
        };

        return FormView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int? id,
        TestimonialFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;

        if (id.HasValue)
        {
            var existing = await sender.Send(
                new GetTestimonialByIdQuery(id.Value),
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
                "Fotoğraf seçiniz.");
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

            var input = new TestimonialInput
            {
                FullName = model.FullName.Trim(),
                ImageUrl = uploadedImageUrl
                    ?? model.ExistingImageUrl
                    ?? string.Empty,
                Rating = model.Rating,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                Translations =
                [
                    new TestimonialTranslationDto
                    {
                        LanguageCode = "tr",
                        Title = model.TurkishTitle.Trim(),
                        Comment = model.TurkishComment.Trim()
                    }
                ]
            };

            if (model.HasEnglishContent())
            {
                input.Translations.Add(new TestimonialTranslationDto
                {
                    LanguageCode = "en",
                    Title = model.EnglishTitle!.Trim(),
                    Comment = model.EnglishComment!.Trim()
                });
            }

            var savedId = await sender.Send(
                new SaveTestimonialCommand(id, input),
                cancellationToken);

            if (!savedId.HasValue)
            {
                return NotFound();
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
                "Referans kaydedilemedi. Id: {TestimonialId}",
                id);

            ModelState.AddModelError(
                string.Empty,
                "Referans kaydedilemedi. Lütfen tekrar deneyiniz.");
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
            ? "Referans güncellendi."
            : "Referans oluşturuldu.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var testimonial = await sender.Send(
            new GetTestimonialByIdQuery(id),
            cancellationToken);

        if (testimonial is null)
        {
            return NotFound();
        }

        try
        {
            var deleted = await sender.Send(
                new DeleteTestimonialCommand(id),
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
                "Referans silinemedi. Id: {TestimonialId}",
                id);

            TempData["Error"] =
                "Referans silinemedi. Lütfen tekrar deneyiniz.";

            return RedirectToAction(nameof(Index));
        }

        TryDeleteImage(testimonial.ImageUrl);

        TempData["Success"] = "Referans silindi.";

        return RedirectToAction(nameof(Index));
    }

    private ViewResult FormView(TestimonialFormViewModel model)
    {
        ViewData["Title"] = model.Id.HasValue
            ? "Referans Düzenle"
            : "Yeni Referans";

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
                "Referans görseli temizlenemedi: {ImageUrl}",
                imageUrl);
        }
    }
}