using System.ComponentModel.DataAnnotations;
using LifeSure.Data;
using LifeSure.Enums;
using LifeSure.Mediator.ContactMessages.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.ContactMessages.Handlers;

public class UpdateContactMessageStatusCommandHandler(
    LifeSureDbContext context)
    : IRequestHandler<UpdateContactMessageStatusCommand, bool>
{
    public async Task<bool> Handle(
        UpdateContactMessageStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(ContactMessageStatus), request.Status))
        {
            throw new ValidationException(
                "Geçerli bir mesaj durumu seçiniz.");
        }

        var message = await context.ContactMessages
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (message is null)
        {
            return false;
        }

        message.Status = request.Status;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}