using LifeSure.Data;
using LifeSure.Entities;
using LifeSure.Mediator.Services.Commands;
using LifeSure.Mediator.Services.Models;
using MediatR;

namespace LifeSure.Mediator.Services.Handlers;

public class CreateServiceCommandHandler(LifeSureDbContext context)
    : IRequestHandler<CreateServiceCommand, int>
{
    public async Task<int> Handle(
        CreateServiceCommand request,
        CancellationToken cancellationToken)
    {
        var input = request.Input;

        ServiceInputValidator.Validate(input);

        var service = new Service
        {
            ImageUrl = input.ImageUrl,
            IconClass = input.IconClass,
            IsActive = input.IsActive,
            DisplayOrder = input.DisplayOrder,

            Translations = input.Translations
                .Select(x => new ServiceTranslation
                {
                    LanguageCode = x.LanguageCode,
                    Title = x.Title,
                    ShortDescription = x.ShortDescription,
                    Description = x.Description
                })
                .ToList()
        };

        context.Services.Add(service);

        await context.SaveChangesAsync(cancellationToken);

        return service.Id;
    }
}