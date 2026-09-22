using System.ComponentModel.DataAnnotations;
using LifeSure.Areas.Admin.Models;
using LifeSure.Mediator.SiteSettings.Commands;
using LifeSure.Mediator.SiteSettings.Models;
using LifeSure.Mediator.SiteSettings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Areas.Admin.Controllers;

public class SiteSettingController(
    ISender sender,
    ILogger<SiteSettingController> logger)
    : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Site Ayarları";

        var setting = await sender.Send(
            new GetSiteSettingQuery(),
            cancellationToken);

        if (setting is null)
        {
            return View(new SiteSettingFormViewModel());
        }

        var turkish = setting.Translations
            .FirstOrDefault(x => x.LanguageCode == "tr");

        var english = setting.Translations
            .FirstOrDefault(x => x.LanguageCode == "en");

        var model = new SiteSettingFormViewModel
        {
            SiteName = setting.SiteName,
            Email = setting.Email,
            PhoneNumber = setting.PhoneNumber,
            Address = setting.Address,
            MapUrl = setting.MapUrl,
            FacebookUrl = setting.FacebookUrl,
            InstagramUrl = setting.InstagramUrl,
            LinkedInUrl = setting.LinkedInUrl,
            XUrl = setting.XUrl,
            TurkishFooterDescription =
                turkish?.FooterDescription ?? string.Empty,
            TurkishMetaDescription =
                turkish?.MetaDescription ?? string.Empty,
            EnglishFooterDescription = english?.FooterDescription,
            EnglishMetaDescription = english?.MetaDescription
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        SiteSettingFormViewModel model,
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Site Ayarları";

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var input = new SiteSettingInput
        {
            SiteName = model.SiteName.Trim(),
            Email = model.Email.Trim(),
            PhoneNumber = model.PhoneNumber.Trim(),
            Address = model.Address.Trim(),
            MapUrl = NormalizeOptional(model.MapUrl),
            FacebookUrl = NormalizeOptional(model.FacebookUrl),
            InstagramUrl = NormalizeOptional(model.InstagramUrl),
            LinkedInUrl = NormalizeOptional(model.LinkedInUrl),
            XUrl = NormalizeOptional(model.XUrl),
            Translations =
            [
                new SiteSettingTranslationDto
                {
                    LanguageCode = "tr",
                    FooterDescription =
                        model.TurkishFooterDescription.Trim(),
                    MetaDescription =
                        model.TurkishMetaDescription.Trim()
                }
            ]
        };

        if (model.HasEnglishContent())
        {
            input.Translations.Add(new SiteSettingTranslationDto
            {
                LanguageCode = "en",
                FooterDescription =
                    model.EnglishFooterDescription!.Trim(),
                MetaDescription =
                    model.EnglishMetaDescription!.Trim()
            });
        }

        try
        {
            await sender.Send(
                new SaveSiteSettingCommand(input),
                cancellationToken);

            TempData["Success"] = "Site ayarları kaydedildi.";

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
                "Site ayarları kaydedilemedi.");

            ModelState.AddModelError(
                string.Empty,
                "Site ayarları kaydedilemedi. Lütfen tekrar deneyiniz.");
        }

        return View("Index", model);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}