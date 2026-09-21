using LifeSure.Data;
using LifeSure.Repositories;

namespace LifeSure.UnitOfWork;

public class EfUnitOfWork(
    LifeSureDbContext context,
    IServiceRepository services)
    : IUnitOfWork
{
    public IServiceRepository Services { get; } = services;

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}