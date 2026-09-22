using System.ComponentModel.DataAnnotations;

namespace LifeSure.Mediator.Testimonials.Models;

public class TestimonialInput
{
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required, MinLength(1)]
    public List<TestimonialTranslationDto> Translations { get; set; } = [];
}

public class TestimonialTranslationDto
{
    [Required]
    [RegularExpression("^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(3000)]
    public string Comment { get; set; } = string.Empty;
}

public static class TestimonialInputValidator
{
    public static void Validate(TestimonialInput input)
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
                "Türkçe unvan ve yorum zorunludur.");
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