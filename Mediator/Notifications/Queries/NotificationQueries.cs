using LifeSure.Mediator.Notifications.Results;
using MediatR;

namespace LifeSure.Mediator.Notifications.Queries;

public record GetNotificationsQuery(
    int Page = 1,
    bool OnlyUnread = false) : IRequest<NotificationListResult>;

public record GetUnreadNotificationCountQuery() : IRequest<int>;