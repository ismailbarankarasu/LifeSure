using LifeSure.Areas.Admin.Models;
using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Faqs.Commands;
using LifeSure.CQRS.Faqs.Models;
using LifeSure.CQRS.Faqs.Queries;
using LifeSure.CQRS.Faqs.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LifeSure.Areas.Admin.Controllers;

public class FaqController(
    IQueryHandler<GetFaqsQuery, List<FaqResult>> listHandler,
    IQueryHandler<GetFaqByIdQuery, FaqResult?> detailHandler,
    ICommandHandler<SaveFaqCommand, int?> saveHandler,
    ICommandHandler<DeleteFaqCommand, bool> deleteHandler,
    ILogger<FaqController> logger)
    : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var faqs = await listHandler.HandleAsync(
            new GetFaqsQuery(),
            cancellationToken);

        return View(faqs);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return FormView(new FaqFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var faq = await detailHandler.HandleAsync(
            new GetFaqByIdQuery(id),
            cancellationToken);

        if (faq is null)
        {
            return NotFound();
        }

        var turkish = faq.Translations
            .FirstOrDefault(x => x.LanguageCode == "tr");

        var english = faq.Translations
            .FirstOrDefault(x => x.LanguageCode == "en");

        var model = new FaqFormViewModel
        {
            Id = faq.Id,
            DisplayOrder = faq.DisplayOrder,
            IsActive = faq.IsActive,
            TurkishQuestion = turkish?.Question ?? string.Empty,
            TurkishAnswer = turkish?.Answer ?? string.Empty,
            EnglishQuestion = english?.Question,
            EnglishAnswer = english?.Answer
        };

        return FormView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int? id,
        FaqFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;

        if (id.HasValue)
        {
            var existing = await detailHandler.HandleAsync(
                new GetFaqByIdQuery(id.Value),
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

        var input = new FaqInput
        {
            IsActive = model.IsActive,
            DisplayOrder = model.DisplayOrder,
            Translations =
            [
                new FaqTranslationDto
                {
                    LanguageCode = "tr",
                    Question = model.TurkishQuestion.Trim(),
                    Answer = model.TurkishAnswer.Trim()
                }
            ]
        };

        if (model.HasEnglishContent())
        {
            input.Translations.Add(new FaqTranslationDto
            {
                LanguageCode = "en",
                Question = model.EnglishQuestion!.Trim(),
                Answer = model.EnglishAnswer!.Trim()
            });
        }

        try
        {
            var savedId = await saveHandler.HandleAsync(
                new SaveFaqCommand(id, input),
                cancellationToken);

            if (!savedId.HasValue)
            {
                return NotFound();
            }

            TempData["Success"] = id.HasValue
                ? "Soru güncellendi."
                : "Soru oluşturuldu.";

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
                "Soru kaydedilemedi. Id: {FaqId}",
                id);

            ModelState.AddModelError(
                string.Empty,
                "Soru kaydedilemedi. Lütfen tekrar deneyiniz.");
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
                new DeleteFaqCommand(id),
                cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["Success"] = "Soru silindi.";
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "Soru silinemedi. Id: {FaqId}",
                id);

            TempData["Error"] =
                "Soru silinemedi. Lütfen tekrar deneyiniz.";
        }

        return RedirectToAction(nameof(Index));
    }

    private ViewResult FormView(FaqFormViewModel model)
    {
        ViewData["Title"] = model.Id.HasValue
            ? "Soru Düzenle"
            : "Yeni Soru";

        return View("Form", model);
    }
}