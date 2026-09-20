using MediatR;

namespace LifeSure.Mediator.Services.Commands;

public record DeleteServiceCommand(int Id) : IRequest<bool>;