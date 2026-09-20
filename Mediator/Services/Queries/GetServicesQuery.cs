using LifeSure.Mediator.Services.Results;
using MediatR;

namespace LifeSure.Mediator.Services.Queries;

public record GetServicesQuery(
    bool OnlyActive = false) : IRequest<List<ServiceResult>>;