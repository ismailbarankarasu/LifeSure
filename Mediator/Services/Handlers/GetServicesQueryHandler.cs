using LifeSure.Data;
using LifeSure.Mediator.Services.Queries;
using LifeSure.Mediator.Services.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.Services.Handlers;

public class GetServicesQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetServicesQuery, List<ServiceResult>>
{
    public async Task<List<ServiceResult>> Handle(
        GetServicesQuery request,
        CancellationToken cancellationToken)
    {
        var servicesQuery = context.Services
            .AsNoTracking()
            .AsQueryable();

        if (request.OnlyActive)
        {
            servicesQuery = servicesQuery.Where(x => x.IsActive);
        }

        var services = await servicesQuery
            .Include(x => x.Translations)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return services
            .Select(ServiceResult.FromEntity)
            .ToList();
    }
}