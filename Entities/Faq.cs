namespace LifeSure.Entities;

public class Faq : DisplayEntity
{
    public ICollection<FaqTranslation> Translations { get; set; }  = new List<FaqTranslation>();
}

public class FaqTranslation : BaseEntity
{
    public int FaqId { get; set; }

    public Faq Faq { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;
}