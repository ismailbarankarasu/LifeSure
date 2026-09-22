using System.ComponentModel.DataAnnotations;

namespace LifeSure.Mediator.SiteSettings.Models;

public class SiteSettingInput
{
    [Required, StringLength(150)]
    public string SiteName { get; set; } = "LifeSure";

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [StringLength(2000)]
    [Url]
    [RegularExpression(@"^https://[^\s\\]+$")]
    public string? MapUrl { get; set; }

    [StringLength(1000)]
    [Url]
    [RegularExpression(@"^https://[^\s\\]+$")]
    public string? FacebookUrl { get; set; }

    [StringLength(1000)]
    [Url]
    [RegularExpression(@"^https://[^\s\\]+$")]
    public string? InstagramUrl { get; set; }

    [StringLength(1000)]
    [Url]
    [RegularExpression(@"^https://[^\s\\]+$")]
    public string? LinkedInUrl { get; set; }

    [StringLength(1000)]
    [Url]
    [RegularExpression(@"^https://[^\s\\]+$")]
    public string? XUrl { get; set; }

    [Required, MinLength(1)]
    public List<SiteSettingTranslationDto> Translations { get; set; } = [];
}

public class SiteSettingTranslationDto
{
    [Required]
    [RegularExpression("^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required, StringLength(2000)]
    public string FooterDescription { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string MetaDescription { get; set; } = string.Empty;
}

public static class SiteSettingInputValidator
{
    public static void Validate(SiteSettingInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Validator.ValidateObject(
            input,
            new ValidationContext(input),
            validateAllProperties: true);

        foreach (var translation in input.Translations)
        {
            if (translation is null)
            {
                throw new ValidationException(
                    "Çeviri bilgileri boş olamaz.");
            }

            Validator.ValidateObject(
                translation,
                new ValidationContext(translation),
                validateAllProperties: true);
        }

        var languages = input.Translations
            .Select(x => x.LanguageCode)
            .ToList();

        if (languages.Distinct().Count() != languages.Count)
        {
            throw new ValidationException(
                "Aynı dil için birden fazla çeviri eklenemez.");
        }

        if (!languages.Contains("tr"))
        {
            throw new ValidationException(
                "Türkçe açıklamalar zorunludur.");
        }
    }
}