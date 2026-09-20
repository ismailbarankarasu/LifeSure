using System.ComponentModel.DataAnnotations;

namespace LifeSure.CQRS.Sliders.Models;

public static class SliderInputValidator
{
    public static void Validate(SliderInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Validator.ValidateObject(input, new ValidationContext(input), validateAllProperties: true);

        foreach (var translation in input.Translations)
        {
            if (translation is null)
            {
                throw new ValidationException("Çeviri bilgisi boş olamaz.");
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
            throw new ValidationException("Aynı dil için birden fazla çeviri eklenemez.");
        }

        if (!languages.Contains("tr"))
        {
            throw new ValidationException("Türkçe çeviri zorunludur.");
        }

        if (!IsLocalPath(input.ImageUrl))
        {
            throw new ValidationException("Görsel yolu / ile başlayan yerel bir yol olmalıdır.");
        }

        if (!IsLocalPath(input.ButtonUrl))
        {
            throw new ValidationException("Buton bağlantısı / ile başlayan site içi bir yol olmalıdır.");
        }
    }

    private static bool IsLocalPath(string value)
    {
        return value.StartsWith('/')
            && !value.StartsWith("//")
            && !value.Contains('\\')
            && !value.Any(char.IsControl);
    }
}