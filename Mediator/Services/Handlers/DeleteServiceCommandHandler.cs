using LifeSure.Mediator.Services.Commands;
using LifeSure.UnitOfWork;
using MediatR;

namespace LifeSure.Mediator.Services.Handlers;

public class DeleteServiceCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteServiceCommand, bool>
{
    public async Task<bool> Handle(
        DeleteServiceCommand request,
        CancellationToken cancellationToken)
    {
        var service = await unitOfWork.Services
            .GetWithTranslationsAsync(
                request.Id,
                cancellationToken);

        if (service is null)
        {
            return false;
        }

        unitOfWork.Services.Remove(service);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}