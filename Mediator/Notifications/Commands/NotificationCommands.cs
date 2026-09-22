using MediatR;

namespace LifeSure.Mediator.Notifications.Commands;

public record OpenNotificationCommand(
    int Id) : IRequest<int?>;

public record MarkAllNotificationsAsReadCommand() : IRequest<int>;