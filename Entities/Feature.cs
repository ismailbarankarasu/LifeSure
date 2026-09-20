namespace LifeSure.Entities;

public class Feature : DisplayEntity
{
    public string IconClass { get; set; } = string.Empty;

    public ICollection<FeatureTranslation> Translations { get; set; } = new List<FeatureTranslation>();
}

public class FeatureTranslation : BaseEntity
{
    public int FeatureId { get; set; }

    public Feature Feature { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}