namespace LifeSure.Entities;

public class About : DisplayEntity
{
    public string ImageUrl { get; set; } = string.Empty;

    public ICollection<AboutTranslation> Translations { get; set; } = new List<AboutTranslation>();
}

public class AboutTranslation : BaseEntity
{
    public int AboutId { get; set; }

    public About About { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string Subtitle { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}