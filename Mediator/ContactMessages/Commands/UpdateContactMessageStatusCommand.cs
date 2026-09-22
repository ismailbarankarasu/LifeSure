using LifeSure.Enums;
using MediatR;

namespace LifeSure.Mediator.ContactMessages.Commands;

public record UpdateContactMessageStatusCommand(
    int Id,
    ContactMessageStatus Status) : IRequest<bool>;