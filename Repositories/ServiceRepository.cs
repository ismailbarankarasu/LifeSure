using LifeSure.Data;
using LifeSure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Repositories;

public class ServiceRepository(LifeSureDbContext context)
    : IServiceRepository
{
    public Task<Service?> GetWithTranslationsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return context.Services
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public void Add(Service service)
    {
        context.Services.Add(service);
    }

    public void Remove(Service service)
    {
        context.Services.Remove(service);
    }
}