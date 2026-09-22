using LifeSure.Data;
using LifeSure.Mediator.Notifications.Queries;
using LifeSure.Mediator.Notifications.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.Notifications.Handlers;

public class GetNotificationsQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetNotificationsQuery, NotificationListResult>
{
    public async Task<NotificationListResult> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        const int pageSize = 20;

        var unreadCount = await context.Notifications
            .CountAsync(x => !x.IsRead, cancellationToken);

        var query = context.Notifications
            .AsNoTracking()
            .AsQueryable();

        if (request.OnlyUnread)
        {
            query = query.Where(x => !x.IsRead);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(totalCount / (double)pageSize));

        var page = Math.Clamp(request.Page, 1, totalPages);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new NotificationListItem(
                x.Id,
                x.Title,
                x.Description,
                x.IsRead,
                x.CreatedAt,
                x.ReadAt))
            .ToListAsync(cancellationToken);

        return new NotificationListResult(
            items,
            page,
            totalPages,
            totalCount,
            unreadCount,
            request.OnlyUnread);
    }
}

public class GetUnreadNotificationCountQueryHandler(
    LifeSureDbContext context)
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    public Task<int> Handle(
        GetUnreadNotificationCountQuery request,
        CancellationToken cancellationToken)
    {
        return context.Notifications
            .CountAsync(x => !x.IsRead, cancellationToken);
    }
}