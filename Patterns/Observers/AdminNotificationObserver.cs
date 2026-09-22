using LifeSure.Data;
using LifeSure.Entities;

namespace LifeSure.Patterns.Observers;

public class AdminNotificationObserver(LifeSureDbContext context)
    : IContactMessageObserver
{
    public Task OnMessageCreatedAsync(
        ContactMessage message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var notification = new Notification
        {
            ContactMessage = message,
            Title = "Yeni iletişim mesajı",
            Description =
                $"{message.FullName} adlı kişiden yeni mesaj: {message.Subject}",
            IsRead = false
        };

        context.Notifications.Add(notification);

        return Task.CompletedTask;
    }
}