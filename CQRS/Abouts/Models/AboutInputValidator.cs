using LifeSure.CQRS.Abouts.Models;
using System.ComponentModel.DataAnnotations;

public static class AboutInputValidator
{
    public static void Validate(AboutInput input)
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

        if (languages.Distinct(StringComparer.OrdinalIgnoreCase).Count()
            != languages.Count)
        {
            throw new ValidationException(
                "Aynı dil için birden fazla çeviri eklenemez.");
        }

        if (!languages.Contains("tr"))
        {
            throw new ValidationException(
                "Türkçe içerik zorunludur.");
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