namespace LifeSure.Entities;

public class Notification : BaseEntity
{
    public int ContactMessageId { get; set; }

    public ContactMessage ContactMessage { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }
}