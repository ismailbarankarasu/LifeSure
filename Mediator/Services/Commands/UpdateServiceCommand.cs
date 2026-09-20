using LifeSure.Mediator.Services.Models;
using MediatR;

namespace LifeSure.Mediator.Services.Commands;

public record UpdateServiceCommand(int Id, ServiceInput Input) : IRequest<bool>;