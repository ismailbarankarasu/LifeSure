using System.ComponentModel.DataAnnotations;
using LifeSure.Areas.Admin.Models;
using LifeSure.Mediator.TeamMembers.Commands;
using LifeSure.Mediator.TeamMembers.Models;
using LifeSure.Mediator.TeamMembers.Queries;
using LifeSure.Services.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Areas.Admin.Controllers;

public class TeamMemberController(
    ISender sender,
    IImageStorageService imageStorage,
    ILogger<TeamMemberController> logger)
    : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var members = await sender.Send(
            new GetTeamMembersQuery(),
            cancellationToken);

        return View(members);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return FormView(new TeamMemberFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var member = await sender.Send(
            new GetTeamMemberByIdQuery(id),
            cancellationToken);

        if (member is null)
        {
            return NotFound();
        }

        var model = new TeamMemberFormViewModel
        {
            Id = member.Id,
            FullName = member.FullName,
            ExistingImageUrl = member.ImageUrl,
            FacebookUrl = member.FacebookUrl,
            InstagramUrl = member.InstagramUrl,
            LinkedInUrl = member.LinkedInUrl,
            XUrl = member.XUrl,
            DisplayOrder = member.DisplayOrder,
            IsActive = member.IsActive,
            TurkishJobTitle = member.Translations
                .FirstOrDefault(x => x.LanguageCode == "tr")
                ?.JobTitle ?? string.Empty,
            EnglishJobTitle = member.Translations
                .FirstOrDefault(x => x.LanguageCode == "en")
                ?.JobTitle
        };

        return FormView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int? id,
        TeamMemberFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;

        if (id.HasValue)
        {
            var existing = await sender.Send(
                new GetTeamMemberByIdQuery(id.Value),
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
                "Ekip üyesinin fotoğrafını seçiniz.");
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

            var input = new TeamMemberInput
            {
                FullName = model.FullName.Trim(),
                ImageUrl = uploadedImageUrl
                    ?? model.ExistingImageUrl
                    ?? string.Empty,
                FacebookUrl = NormalizeUrl(model.FacebookUrl),
                InstagramUrl = NormalizeUrl(model.InstagramUrl),
                LinkedInUrl = NormalizeUrl(model.LinkedInUrl),
                XUrl = NormalizeUrl(model.XUrl),
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                Translations =
                [
                    new TeamMemberTranslationDto
                    {
                        LanguageCode = "tr",
                        JobTitle = model.TurkishJobTitle.Trim()
                    }
                ]
            };

            if (!string.IsNullOrWhiteSpace(model.EnglishJobTitle))
            {
                input.Translations.Add(new TeamMemberTranslationDto
                {
                    LanguageCode = "en",
                    JobTitle = model.EnglishJobTitle.Trim()
                });
            }

            var savedId = await sender.Send(
                new SaveTeamMemberCommand(id, input),
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
                "Ekip üyesi kaydedilemedi. Id: {TeamMemberId}",
                id);

            ModelState.AddModelError(
                string.Empty,
                "Ekip üyesi kaydedilemedi. Lütfen tekrar deneyiniz.");
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
            ? "Ekip üyesi güncellendi."
            : "Ekip üyesi oluşturuldu.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var member = await sender.Send(
            new GetTeamMemberByIdQuery(id),
            cancellationToken);

        if (member is null)
        {
            return NotFound();
        }

        try
        {
            var deleted = await sender.Send(
                new DeleteTeamMemberCommand(id),
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
                "Ekip üyesi silinemedi. Id: {TeamMemberId}",
                id);

            TempData["Error"] =
                "Ekip üyesi silinemedi. Lütfen tekrar deneyiniz.";

            return RedirectToAction(nameof(Index));
        }

        TryDeleteImage(member.ImageUrl);

        TempData["Success"] = "Ekip üyesi silindi.";

        return RedirectToAction(nameof(Index));
    }

    private ViewResult FormView(TeamMemberFormViewModel model)
    {
        ViewData["Title"] = model.Id.HasValue
            ? "Ekip Üyesi Düzenle"
            : "Yeni Ekip Üyesi";

        return View("Form", model);
    }

    private static string? NormalizeUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
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
                "Ekip üyesi görseli temizlenemedi: {ImageUrl}",
                imageUrl);
        }
    }
}