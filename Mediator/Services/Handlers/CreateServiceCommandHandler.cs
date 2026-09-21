using LifeSure.Entities;
using LifeSure.Mediator.Services.Commands;
using LifeSure.Mediator.Services.Models;
using LifeSure.UnitOfWork;
using MediatR;

namespace LifeSure.Mediator.Services.Handlers;

public class CreateServiceCommandHandler(IUnitOfWork unitOfWork)
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
                    Title = x.Title.Trim(),
                    ShortDescription = x.ShortDescription.Trim(),
                    Description = x.Description.Trim()
                })
                .ToList()
        };

        unitOfWork.Services.Add(service);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return service.Id;
    }
}