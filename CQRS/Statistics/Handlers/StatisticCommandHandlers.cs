using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Statistics.Commands;
using LifeSure.CQRS.Statistics.Models;
using LifeSure.Data;
using LifeSure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Statistics.Handlers;

public class SaveStatisticCommandHandler(LifeSureDbContext context)
    : ICommandHandler<SaveStatisticCommand, int?>
{
    public async Task<int?> HandleAsync(
        SaveStatisticCommand command,
        CancellationToken cancellationToken = default)
    {
        var input = command.Input;

        StatisticInputValidator.Validate(input);

        Statistic statistic;

        if (command.Id.HasValue)
        {
            var existing = await context.Statistics
                .Include(x => x.Translations)
                .SingleOrDefaultAsync(
                    x => x.Id == command.Id.Value,
                    cancellationToken);

            if (existing is null)
            {
                return null;
            }

            statistic = existing;
        }
        else
        {
            statistic = new Statistic();
            context.Statistics.Add(statistic);
        }

        statistic.Source = input.Source;
        statistic.Value = input.Value;
        statistic.Suffix = string.IsNullOrWhiteSpace(input.Suffix)
            ? null
            : input.Suffix.Trim();

        statistic.IsActive = input.IsActive;
        statistic.DisplayOrder = input.DisplayOrder;

        foreach (var translationInput in input.Translations)
        {
            var translation = statistic.Translations
                .FirstOrDefault(
                    x => x.LanguageCode == translationInput.LanguageCode);

            if (translation is null)
            {
                translation = new StatisticTranslation
                {
                    LanguageCode = translationInput.LanguageCode
                };

                statistic.Translations.Add(translation);
            }

            translation.Title = translationInput.Title.Trim();
        }

        // Formda İngilizce başlık temizlenirse mevcut çeviriyi kaldır.
        var submittedLanguages = input.Translations
            .Select(x => x.LanguageCode)
            .ToHashSet();

        var removedTranslations = statistic.Translations
            .Where(x => !submittedLanguages.Contains(x.LanguageCode))
            .ToList();

        context.StatisticTranslations.RemoveRange(removedTranslations);

        await context.SaveChangesAsync(cancellationToken);

        return statistic.Id;
    }
}

public class DeleteStatisticCommandHandler(LifeSureDbContext context)
    : ICommandHandler<DeleteStatisticCommand, bool>
{
    public async Task<bool> HandleAsync(
        DeleteStatisticCommand command,
        CancellationToken cancellationToken = default)
    {
        var statistic = await context.Statistics
            .SingleOrDefaultAsync(
                x => x.Id == command.Id,
                cancellationToken);

        if (statistic is null)
        {
            return false;
        }

        context.Statistics.Remove(statistic);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}