namespace LifeSure.Entities;

public class Slider : DisplayEntity
{
    public string ImageUrl { get; set; } = string.Empty;

    public string? VideoId { get; set; }

    public string ButtonUrl { get; set; } = "/#hizmetler";

    public ICollection<SliderTranslation> Translations { get; set; } = new List<SliderTranslation>();
}

public class SliderTranslation : BaseEntity
{
    public int SliderId { get; set; }

    public Slider Slider { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string Subtitle { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ButtonText { get; set; } = string.Empty;
}