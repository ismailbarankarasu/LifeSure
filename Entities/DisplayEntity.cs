namespace LifeSure.Entities;

public abstract class DisplayEntity : BaseEntity
{
    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}