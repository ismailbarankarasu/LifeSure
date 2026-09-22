using LifeSure.Mediator.ContactMessages.Models;
using MediatR;

namespace LifeSure.Mediator.ContactMessages.Commands;

public record CreateContactMessageCommand(
    ContactMessageInput Input) : IRequest<int>;