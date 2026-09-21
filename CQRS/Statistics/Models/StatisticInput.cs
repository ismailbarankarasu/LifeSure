using System.ComponentModel.DataAnnotations;
using LifeSure.Enums;

namespace LifeSure.CQRS.Statistics.Models;

public class StatisticInput
{
    public StatisticSource Source { get; set; }

    [Range(0, int.MaxValue)]
    public int Value { get; set; }

    [StringLength(20)]
    public string? Suffix { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    [Required, MinLength(1)]
    public List<StatisticTranslationDto> Translations { get; set; } = [];
}

public class StatisticTranslationDto
{
    [Required]
    [RegularExpression("^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;
}

public static class StatisticInputValidator
{
    public static void Validate(StatisticInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Validator.ValidateObject(
            input,
            new ValidationContext(input),
            validateAllProperties: true);

        if (!Enum.IsDefined(typeof(StatisticSource), input.Source))
        {
            throw new ValidationException(
                "Geçerli bir istatistik kaynağı seçiniz.");
        }

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
                "Türkçe başlık zorunludur.");
        }
    }
}