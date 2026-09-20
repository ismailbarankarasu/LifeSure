namespace LifeSure.Entities;

public class Service : DisplayEntity
{
    public string ImageUrl { get; set; } = string.Empty;

    public string IconClass { get; set; } = string.Empty;

    public ICollection<ServiceTranslation> Translations { get; set; } = new List<ServiceTranslation>();
}

public class ServiceTranslation : BaseEntity
{
    public int ServiceId { get; set; }

    public Service Service { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}