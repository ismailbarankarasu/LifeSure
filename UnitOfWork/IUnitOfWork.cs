using LifeSure.Repositories;

namespace LifeSure.UnitOfWork;

public interface IUnitOfWork
{
    IServiceRepository Services { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}