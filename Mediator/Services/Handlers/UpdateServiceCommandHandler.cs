using LifeSure.Data;
using LifeSure.Entities;
using LifeSure.Mediator.Services.Commands;
using LifeSure.Mediator.Services.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.Services.Handlers;

public class UpdateServiceCommandHandler(LifeSureDbContext context)
    : IRequestHandler<UpdateServiceCommand, bool>
{
    public async Task<bool> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var input = request.Input;

        ServiceInputValidator.Validate(input);

        var service = await context.Services
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (service is null)
        {
            return false;
        }

        service.ImageUrl = input.ImageUrl;
        service.IconClass = input.IconClass;
        service.IsActive = input.IsActive;
        service.DisplayOrder = input.DisplayOrder;

        foreach (var item in input.Translations)
        {
            var translation = service.Translations
                .SingleOrDefault(
                    x => x.LanguageCode == item.LanguageCode);

            if (translation is null)
            {
                translation = new ServiceTranslation
                {
                    LanguageCode = item.LanguageCode
                };

                service.Translations.Add(translation);
            }

            translation.Title = item.Title;
            translation.ShortDescription = item.ShortDescription;
            translation.Description = item.Description;
        }

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}