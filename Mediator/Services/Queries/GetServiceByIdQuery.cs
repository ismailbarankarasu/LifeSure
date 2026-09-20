using LifeSure.Mediator.Services.Results;
using MediatR;

namespace LifeSure.Mediator.Services.Queries;

public record GetServiceByIdQuery(int Id) : IRequest<ServiceResult?>;