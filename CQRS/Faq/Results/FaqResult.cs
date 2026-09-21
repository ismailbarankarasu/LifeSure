using LifeSure.CQRS.Faqs.Models;
using LifeSure.Entities;

namespace LifeSure.CQRS.Faqs.Results;

public record FaqResult(
    int Id,
    bool IsActive,
    int DisplayOrder,
    IReadOnlyList<FaqTranslationDto> Translations)
{
    public static FaqResult FromEntity(Faq faq)
    {
        return new FaqResult(
            faq.Id,
            faq.IsActive,
            faq.DisplayOrder,
            faq.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new FaqTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Question = x.Question,
                    Answer = x.Answer
                })
                .ToList());
    }
}