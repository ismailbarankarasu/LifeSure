using System.ComponentModel.DataAnnotations;

namespace LifeSure.Mediator.TeamMembers.Models;

public class TeamMemberInput
{
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

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

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required, MinLength(1)]
    public List<TeamMemberTranslationDto> Translations { get; set; } = [];
}

public class TeamMemberTranslationDto
{
    [Required]
    [RegularExpression("^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required, StringLength(150)]
    public string JobTitle { get; set; } = string.Empty;
}

public static class TeamMemberInputValidator
{
    public static void Validate(TeamMemberInput input)
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
                "Türkçe görev bilgisi zorunludur.");
        }

        if (!input.ImageUrl.StartsWith("/")
            || input.ImageUrl.StartsWith("//")
            || input.ImageUrl.Contains('\\')
            || input.ImageUrl.Any(char.IsControl))
        {
            throw new ValidationException(
                "Görsel yolu geçerli bir yerel dosya yolu olmalıdır.");
        }
    }
}