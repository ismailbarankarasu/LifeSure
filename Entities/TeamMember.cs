namespace LifeSure.Entities;

public class TeamMember : DisplayEntity
{
    public string FullName { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string? FacebookUrl { get; set; }

    public string? InstagramUrl { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? XUrl { get; set; }

    public ICollection<TeamMemberTranslation> Translations { get; set; } = new List<TeamMemberTranslation>();
}

public class TeamMemberTranslation : BaseEntity
{
    public int TeamMemberId { get; set; }

    public TeamMember TeamMember { get; set; } = null!;

    public string LanguageCode { get; set; } = "tr";

    public string JobTitle { get; set; } = string.Empty;
}