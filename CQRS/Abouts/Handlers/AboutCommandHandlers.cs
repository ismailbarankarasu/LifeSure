using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Abouts.Commands;
using LifeSure.CQRS.Abouts.Models;
using LifeSure.Data;
using LifeSure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Abouts.Handlers;

public class SaveAboutCommandHandler(LifeSureDbContext context)
    : ICommandHandler<SaveAboutCommand, int?>
{
    public async Task<int?> HandleAsync(
        SaveAboutCommand command,
        CancellationToken cancellationToken = default)
    {
        var input = command.Input;

        AboutInputValidator.Validate(input);

        About about;

        if (command.Id.HasValue)
        {
            var existing = await context.Abouts
                .Include(x => x.Translations)
                .SingleOrDefaultAsync(
                    x => x.Id == command.Id.Value,
                    cancellationToken);

            if (existing is null)
            {
                return null;
            }

            about = existing;
        }
        else
        {
            about = new About();
            context.Abouts.Add(about);
        }

        about.ImageUrl = input.ImageUrl;
        about.IsActive = input.IsActive;
        about.DisplayOrder = input.DisplayOrder;

        foreach (var item in input.Translations)
        {
            var translation = about.Translations
                .SingleOrDefault(
                    x => x.LanguageCode == item.LanguageCode);

            if (translation is null)
            {
                translation = new AboutTranslation
                {
                    LanguageCode = item.LanguageCode
                };

                about.Translations.Add(translation);
            }

            translation.Subtitle = item.Subtitle;
            translation.Title = item.Title;
            translation.Description = item.Description;
        }

        await context.SaveChangesAsync(cancellationToken);

        return about.Id;
    }
}

public class DeleteAboutCommandHandler(LifeSureDbContext context)
    : ICommandHandler<DeleteAboutCommand, bool>
{
    public async Task<bool> HandleAsync(
        DeleteAboutCommand command,
        CancellationToken cancellationToken = default)
    {
        var about = await context.Abouts.SingleOrDefaultAsync(
            x => x.Id == command.Id,
            cancellationToken);

        if (about is null)
        {
            return false;
        }

        context.Abouts.Remove(about);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}