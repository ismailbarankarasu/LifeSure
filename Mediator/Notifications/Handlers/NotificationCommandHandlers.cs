using LifeSure.Data;
using LifeSure.Mediator.Notifications.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.Notifications.Handlers;

public class OpenNotificationCommandHandler(LifeSureDbContext context)
    : IRequestHandler<OpenNotificationCommand, int?>
{
    public async Task<int?> Handle(
        OpenNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification = await context.Notifications
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (notification is null)
        {
            return null;
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);
        }

        return notification.ContactMessageId;
    }
}

public class MarkAllNotificationsAsReadCommandHandler(
    LifeSureDbContext context)
    : IRequestHandler<MarkAllNotificationsAsReadCommand, int>
{
    public async Task<int> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await context.Notifications
            .Where(x => !x.IsRead)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.IsRead, true)
                    .SetProperty(x => x.ReadAt, (DateTime?)now)
                    .SetProperty(x => x.UpdatedAt, (DateTime?)now),
                cancellationToken);
    }
}