using LifeSure.Areas.Admin.Models;
using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Statistics.Commands;
using LifeSure.CQRS.Statistics.Models;
using LifeSure.CQRS.Statistics.Queries;
using LifeSure.CQRS.Statistics.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LifeSure.Areas.Admin.Controllers;

public class StatisticController(
    IQueryHandler<GetStatisticsQuery, List<StatisticResult>> listHandler,
    IQueryHandler<GetStatisticByIdQuery, StatisticResult?> detailHandler,
    ICommandHandler<SaveStatisticCommand, int?> saveHandler,
    ICommandHandler<DeleteStatisticCommand, bool> deleteHandler,
    ILogger<StatisticController> logger)
    : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var statistics = await listHandler.HandleAsync(
            new GetStatisticsQuery(),
            cancellationToken);

        return View(statistics);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return FormView(new StatisticFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var statistic = await detailHandler.HandleAsync(
            new GetStatisticByIdQuery(id),
            cancellationToken);

        if (statistic is null)
        {
            return NotFound();
        }

        var model = new StatisticFormViewModel
        {
            Id = statistic.Id,
            Source = statistic.Source,
            Value = statistic.Value,
            Suffix = statistic.Suffix,
            DisplayOrder = statistic.DisplayOrder,
            IsActive = statistic.IsActive,
            TurkishTitle = statistic.Translations
                .FirstOrDefault(x => x.LanguageCode == "tr")
                ?.Title ?? string.Empty,
            EnglishTitle = statistic.Translations
                .FirstOrDefault(x => x.LanguageCode == "en")
                ?.Title
        };

        return FormView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int? id,
        StatisticFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;

        if (id.HasValue)
        {
            var existing = await detailHandler.HandleAsync(
                new GetStatisticByIdQuery(id.Value),
                cancellationToken);

            if (existing is null)
            {
                return NotFound();
            }
        }

        if (!ModelState.IsValid)
        {
            return FormView(model);
        }

        var input = new StatisticInput
        {
            Source = model.Source,
            Value = model.Value,
            Suffix = model.Suffix,
            DisplayOrder = model.DisplayOrder,
            IsActive = model.IsActive,
            Translations =
            [
                new StatisticTranslationDto
                {
                    LanguageCode = "tr",
                    Title = model.TurkishTitle.Trim()
                }
            ]
        };

        if (!string.IsNullOrWhiteSpace(model.EnglishTitle))
        {
            input.Translations.Add(new StatisticTranslationDto
            {
                LanguageCode = "en",
                Title = model.EnglishTitle.Trim()
            });
        }

        try
        {
            var savedId = await saveHandler.HandleAsync(
                new SaveStatisticCommand(id, input),
                cancellationToken);

            if (!savedId.HasValue)
            {
                return NotFound();
            }

            TempData["Success"] = id.HasValue
                ? "İstatistik güncellendi."
                : "İstatistik oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "İstatistik kaydedilemedi. Id: {StatisticId}",
                id);

            ModelState.AddModelError(
                string.Empty,
                "İstatistik kaydedilemedi. Lütfen tekrar deneyiniz.");
        }

        return FormView(model);
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
                new DeleteStatisticCommand(id),
                cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["Success"] = "İstatistik silindi.";
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "İstatistik silinemedi. Id: {StatisticId}",
                id);

            TempData["Error"] =
                "İstatistik silinemedi. Lütfen tekrar deneyiniz.";
        }

        return RedirectToAction(nameof(Index));
    }

    private ViewResult FormView(StatisticFormViewModel model)
    {
        ViewData["Title"] = model.Id.HasValue
            ? "İstatistik Düzenle"
            : "Yeni İstatistik";

        return View("Form", model);
    }
}