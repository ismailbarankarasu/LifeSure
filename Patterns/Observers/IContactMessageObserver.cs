using LifeSure.Entities;

namespace LifeSure.Patterns.Observers;

public interface IContactMessageObserver
{
    Task OnMessageCreatedAsync(
        ContactMessage message,
        CancellationToken cancellationToken = default);
}