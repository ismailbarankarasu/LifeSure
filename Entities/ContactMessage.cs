using LifeSure.Enums;

namespace LifeSure.Entities;

public class ContactMessage : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public ContactMessageStatus Status { get; set; } = ContactMessageStatus.New;
}