namespace LifeSure.Entities;

public class SiteSetting : BaseEntity
{
    public string SiteName { get; set; } = "LifeSure";

    public string? LogoUrl { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? MapUrl { get; set; }

    public string? FacebookUrl { get; set; }

    public string? InstagramUrl { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? XUrl { get; set; }

    public ICollection<SiteSettingTranslation> Translations { get; set; } = new List<SiteSettingTranslation>();
}

public class SiteSettingTranslation : BaseEntity
{
    public int SiteSettingId { get; set; }

    public SiteSetting SiteSetting { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string FooterDescription { get; set; } = string.Empty;

    public string MetaDescription { get; set; } = string.Empty;
}