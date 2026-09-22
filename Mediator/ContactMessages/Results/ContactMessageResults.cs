using LifeSure.Enums;

namespace LifeSure.Mediator.ContactMessages.Results;

public record ContactMessageListItem(
    int Id,
    string FullName,
    string Subject,
    ContactMessageStatus Status,
    DateTime CreatedAt);

public record ContactMessageListResult(
    IReadOnlyList<ContactMessageListItem> Items,
    int Page,
    int TotalPages,
    int TotalCount);

public record ContactMessageDetailResult(
    int Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string Subject,
    string Message,
    ContactMessageStatus Status,
    DateTime CreatedAt);