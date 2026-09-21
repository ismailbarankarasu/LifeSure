using System.ComponentModel.DataAnnotations;

namespace LifeSure.CQRS.Faqs.Models;

public class FaqInput
{
    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required, MinLength(1)]
    public List<FaqTranslationDto> Translations { get; set; } = [];
}

public class FaqTranslationDto
{
    [Required]
    [RegularExpression("^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required, StringLength(500)]
    public string Question { get; set; } = string.Empty;

    [Required, StringLength(6000)]
    public string Answer { get; set; } = string.Empty;
}

public static class FaqInputValidator
{
    public static void Validate(FaqInput input)
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
                "Türkçe soru ve cevap zorunludur.");
        }
    }
}