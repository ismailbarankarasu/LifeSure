using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Features.Commands;
using LifeSure.CQRS.Features.Models;
using LifeSure.Data;
using LifeSure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Features.Handlers;

public class CreateFeatureCommandHandler(LifeSureDbContext context)
    : ICommandHandler<CreateFeatureCommand, int>
{
    public async Task<int> HandleAsync(
        CreateFeatureCommand command,
        CancellationToken cancellationToken = default)
    {
        var input = command.Input;

        FeatureInputValidator.Validate(input);

        var feature = new Feature
        {
            IconClass = input.IconClass,
            IsActive = input.IsActive,
            DisplayOrder = input.DisplayOrder,

            Translations = input.Translations
                .Select(x => new FeatureTranslation
                {
                    LanguageCode = x.LanguageCode,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToList()
        };

        context.Features.Add(feature);

        await context.SaveChangesAsync(cancellationToken);

        return feature.Id;
    }
}

public class UpdateFeatureCommandHandler(LifeSureDbContext context)
    : ICommandHandler<UpdateFeatureCommand, bool>
{
    public async Task<bool> HandleAsync(
        UpdateFeatureCommand command,
        CancellationToken cancellationToken = default)
    {
        var input = command.Input;

        FeatureInputValidator.Validate(input);

        var feature = await context.Features
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == command.Id,
                cancellationToken);

        if (feature is null)
        {
            return false;
        }

        feature.IconClass = input.IconClass;
        feature.IsActive = input.IsActive;
        feature.DisplayOrder = input.DisplayOrder;

        foreach (var item in input.Translations)
        {
            var translation = feature.Translations
                .SingleOrDefault(
                    x => x.LanguageCode == item.LanguageCode);

            if (translation is null)
            {
                translation = new FeatureTranslation
                {
                    LanguageCode = item.LanguageCode
                };

                feature.Translations.Add(translation);
            }

            translation.Title = item.Title;
            translation.Description = item.Description;
        }

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class DeleteFeatureCommandHandler(LifeSureDbContext context)
    : ICommandHandler<DeleteFeatureCommand, bool>
{
    public async Task<bool> HandleAsync(
        DeleteFeatureCommand command,
        CancellationToken cancellationToken = default)
    {
        var feature = await context.Features.SingleOrDefaultAsync(
            x => x.Id == command.Id,
            cancellationToken);

        if (feature is null)
        {
            return false;
        }

        context.Features.Remove(feature);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}