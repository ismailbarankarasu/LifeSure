using LifeSure.Data;
using LifeSure.Mediator.Services.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.Services.Handlers;

public class DeleteServiceCommandHandler(LifeSureDbContext context)
    : IRequestHandler<DeleteServiceCommand, bool>
{
    public async Task<bool> Handle(
        DeleteServiceCommand request,
        CancellationToken cancellationToken)
    {
        var service = await context.Services
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (service is null)
        {
            return false;
        }

        context.Services.Remove(service);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}