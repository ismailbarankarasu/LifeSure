using LifeSure.Data;
using LifeSure.Mediator.Services.Queries;
using LifeSure.Mediator.Services.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.Services.Handlers;

public class GetServiceByIdQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetServiceByIdQuery, ServiceResult?>
{
    public async Task<ServiceResult?> Handle(
        GetServiceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var service = await context.Services
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        return service is null
            ? null
            : ServiceResult.FromEntity(service);
    }
}