using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Faqs.Commands;
using LifeSure.CQRS.Faqs.Models;
using LifeSure.Data;
using LifeSure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Faqs.Handlers;

public class SaveFaqCommandHandler(LifeSureDbContext context)
    : ICommandHandler<SaveFaqCommand, int?>
{
    public async Task<int?> HandleAsync(
        SaveFaqCommand command,
        CancellationToken cancellationToken = default)
    {
        var input = command.Input;

        FaqInputValidator.Validate(input);

        Faq faq;

        if (command.Id.HasValue)
        {
            var existing = await context.Faqs
                .Include(x => x.Translations)
                .SingleOrDefaultAsync(
                    x => x.Id == command.Id.Value,
                    cancellationToken);

            if (existing is null)
            {
                return null;
            }

            faq = existing;
        }
        else
        {
            faq = new Faq();
            context.Faqs.Add(faq);
        }

        faq.IsActive = input.IsActive;
        faq.DisplayOrder = input.DisplayOrder;

        foreach (var translationInput in input.Translations)
        {
            var translation = faq.Translations
                .FirstOrDefault(
                    x => x.LanguageCode == translationInput.LanguageCode);

            if (translation is null)
            {
                translation = new FaqTranslation
                {
                    LanguageCode = translationInput.LanguageCode
                };

                faq.Translations.Add(translation);
            }

            translation.Question = translationInput.Question.Trim();
            translation.Answer = translationInput.Answer.Trim();
        }

        // Formda kaldırılan çevirileri veritabanından da kaldır.
        var submittedLanguages = input.Translations
            .Select(x => x.LanguageCode)
            .ToHashSet();

        var removedTranslations = faq.Translations
            .Where(x => !submittedLanguages.Contains(x.LanguageCode))
            .ToList();

        context.FaqTranslations.RemoveRange(removedTranslations);

        await context.SaveChangesAsync(cancellationToken);

        return faq.Id;
    }
}

public class DeleteFaqCommandHandler(LifeSureDbContext context)
    : ICommandHandler<DeleteFaqCommand, bool>
{
    public async Task<bool> HandleAsync(
        DeleteFaqCommand command,
        CancellationToken cancellationToken = default)
    {
        var faq = await context.Faqs
            .SingleOrDefaultAsync(
                x => x.Id == command.Id,
                cancellationToken);

        if (faq is null)
        {
            return false;
        }

        context.Faqs.Remove(faq);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}