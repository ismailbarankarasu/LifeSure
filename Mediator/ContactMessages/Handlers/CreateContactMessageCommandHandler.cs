using System.ComponentModel.DataAnnotations;
using LifeSure.Data;
using LifeSure.Entities;
using LifeSure.Enums;
using LifeSure.Mediator.ContactMessages.Commands;
using LifeSure.Patterns.Observers;
using MediatR;

namespace LifeSure.Mediator.ContactMessages.Handlers;

public class CreateContactMessageCommandHandler(
    LifeSureDbContext context,
    IContactMessagePublisher publisher)
    : IRequestHandler<CreateContactMessageCommand, int>
{
    public async Task<int> Handle(
        CreateContactMessageCommand request,
        CancellationToken cancellationToken)
    {
        var input = request.Input;

        ArgumentNullException.ThrowIfNull(input);

        Validator.ValidateObject(
            input,
            new ValidationContext(input),
            validateAllProperties: true);

        var message = new ContactMessage
        {
            FullName = input.FullName.Trim(),
            Email = input.Email.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(input.PhoneNumber)
                ? null
                : input.PhoneNumber.Trim(),
            Subject = input.Subject.Trim(),
            Message = input.Message.Trim(),
            Status = ContactMessageStatus.New
        };

        context.ContactMessages.Add(message);

        await publisher.NotifyAsync(message, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return message.Id;
    }
}