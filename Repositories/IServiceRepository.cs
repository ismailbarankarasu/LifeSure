using LifeSure.Entities;

namespace LifeSure.Repositories;

public interface IServiceRepository
{
    Task<Service?> GetWithTranslationsAsync(int id, CancellationToken cancellationToken = default);

    void Add(Service service);

    void Remove(Service service);
}