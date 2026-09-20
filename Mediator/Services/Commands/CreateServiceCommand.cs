using LifeSure.Mediator.Services.Models;
using MediatR;

namespace LifeSure.Mediator.Services.Commands;

public record CreateServiceCommand(ServiceInput Input) : IRequest<int>;