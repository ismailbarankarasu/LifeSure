using LifeSure.Entities;

namespace LifeSure.Patterns.Observers;

public interface IContactMessagePublisher
{
    Task NotifyAsync(
        ContactMessage message,
        CancellationToken cancellationToken = default);
}

public class ContactMessagePublisher(
    IEnumerable<IContactMessageObserver> observers)
    : IContactMessagePublisher
{
    public async Task NotifyAsync(
        ContactMessage message,
        CancellationToken cancellationToken = default)
    {
        foreach (var observer in observers)
        {
            await observer.OnMessageCreatedAsync(
                message,
                cancellationToken);
        }
    }
}