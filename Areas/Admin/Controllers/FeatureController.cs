using System.ComponentModel.DataAnnotations;
using LifeSure.Areas.Admin.Models;
using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Features.Commands;
using LifeSure.CQRS.Features.Models;
using LifeSure.CQRS.Features.Queries;
using LifeSure.CQRS.Features.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Areas.Admin.Controllers;

public class FeatureController(
    IQueryHandler<GetFeaturesQuery, List<FeatureResult>> listHandler,
    IQueryHandler<GetFeatureByIdQuery, FeatureResult?> detailHandler,
    ICommandHandler<CreateFeatureCommand, int> createHandler,
    ICommandHandler<UpdateFeatureCommand, bool> updateHandler,
    ICommandHandler<DeleteFeatureCommand, bool> deleteHandler,
    ILogger<FeatureController> logger) : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var features = await listHandler.HandleAsync(
            new GetFeaturesQuery(),
            cancellationToken);

        return View(features);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return FormView(new FeatureFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var feature = await detailHandler.HandleAsync(
            new GetFeatureByIdQuery(id),
            cancellationToken);

        if (feature is null)
        {
            return NotFound();
        }

        var model = new FeatureFormViewModel
        {
            Id = feature.Id,
            IconClass = feature.IconClass,
            IsActive = feature.IsActive,
            DisplayOrder = feature.DisplayOrder,

            Translations = new[] { "tr", "en" }
                .Select(language =>
                {
                    var translation = feature.Translations
                        .FirstOrDefault(x => x.LanguageCode == language);

                    return new FeatureTranslationFormModel
                    {
                        LanguageCode = language,
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
        FeatureFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (id.HasValue)
        {
            var feature = await detailHandler.HandleAsync(
                new GetFeatureByIdQuery(id.Value),
                cancellationToken);

            if (feature is null)
            {
                return NotFound();
            }

            model.Id = feature.Id;
        }

        if (!ModelState.IsValid)
        {
            return FormView(model);
        }

        var input = new FeatureInput
        {
            IconClass = model.IconClass,
            IsActive = model.IsActive,
            DisplayOrder = model.DisplayOrder,

            Translations = model.Translations
                .Where(x => x.LanguageCode == "tr" || x.HasContent())
                .Select(x => new FeatureTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Title = x.Title ?? string.Empty,
                    Description = x.Description ?? string.Empty
                })
                .ToList()
        };

        try
        {
            if (id.HasValue)
            {
                var updated = await updateHandler.HandleAsync(
                    new UpdateFeatureCommand(id.Value, input),
                    cancellationToken);

                if (!updated)
                {
                    return NotFound();
                }
            }
            else
            {
                await createHandler.HandleAsync(
                    new CreateFeatureCommand(input),
                    cancellationToken);
            }
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            return FormView(model);
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "Özellik kaydedilemedi. FeatureId: {FeatureId}",
                id);

            ModelState.AddModelError(
                string.Empty,
                "Kayıt sırasında bir hata oluştu.");

            return FormView(model);
        }

        TempData["SuccessMessage"] = id.HasValue
            ? "Özellik başarıyla güncellendi."
            : "Özellik başarıyla eklendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await deleteHandler.HandleAsync(
                new DeleteFeatureCommand(id),
                cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Özellik başarıyla silindi.";
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "Özellik silinemedi. FeatureId: {FeatureId}",
                id);

            TempData["ErrorMessage"] = "Özellik silinemedi.";
        }

        return RedirectToAction(nameof(Index));
    }

    private ViewResult FormView(FeatureFormViewModel model)
    {
        model.Translations ??=
        [
            new() { LanguageCode = "tr" },
            new() { LanguageCode = "en" }
        ];

        ViewData["Title"] = model.Id == 0
            ? "Özellik Ekle"
            : "Özellik Düzenle";

        return View("Form", model);
    }
}