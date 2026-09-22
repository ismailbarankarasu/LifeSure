namespace LifeSure.Mediator.Notifications.Results;

public record NotificationListItem(
    int Id,
    string Title,
    string Description,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? ReadAt);

public record NotificationListResult(
    IReadOnlyList<NotificationListItem> Items,
    int Page,
    int TotalPages,
    int TotalCount,
    int UnreadCount,
    bool OnlyUnread);