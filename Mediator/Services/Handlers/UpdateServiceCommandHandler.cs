using LifeSure.Entities;
using LifeSure.Mediator.Services.Commands;
using LifeSure.Mediator.Services.Models;
using LifeSure.UnitOfWork;
using MediatR;

namespace LifeSure.Mediator.Services.Handlers;

public class UpdateServiceCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateServiceCommand, bool>
{
    public async Task<bool> Handle(
        UpdateServiceCommand request,
        CancellationToken cancellationToken)
    {
        var input = request.Input;

        ServiceInputValidator.Validate(input);

        var service = await unitOfWork.Services
            .GetWithTranslationsAsync(
                request.Id,
                cancellationToken);

        if (service is null)
        {
            return false;
        }

        service.ImageUrl = input.ImageUrl;
        service.IconClass = input.IconClass;
        service.IsActive = input.IsActive;
        service.DisplayOrder = input.DisplayOrder;

        foreach (var translationInput in input.Translations)
        {
            var translation = service.Translations
                .FirstOrDefault(
                    x => x.LanguageCode == translationInput.LanguageCode);

            if (translation is null)
            {
                translation = new ServiceTranslation
                {
                    LanguageCode = translationInput.LanguageCode
                };

                service.Translations.Add(translation);
            }

            translation.Title = translationInput.Title.Trim();
            translation.ShortDescription =
                translationInput.ShortDescription.Trim();
            translation.Description =
                translationInput.Description.Trim();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}