using LifeSure.Enums;

namespace LifeSure.Entities;

public class Statistic : DisplayEntity
{
    public StatisticSource Source { get; set; } = StatisticSource.Manual;

    public int Value { get; set; }

    public string? Suffix { get; set; }

    public ICollection<StatisticTranslation> Translations { get; set; } = new List<StatisticTranslation>();
}

public class StatisticTranslation : BaseEntity
{
    public int StatisticId { get; set; }

    public Statistic Statistic { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string Title { get; set; } = string.Empty;
}