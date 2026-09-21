using System.ComponentModel.DataAnnotations;

namespace LifeSure.CQRS.Abouts.Models;

public class AboutInput
{
    [Required]
    [StringLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<AboutTranslationDto> Translations { get; set; } = [];
}

public class AboutTranslationDto
{
    [Required]
    [RegularExpression(@"^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required(ErrorMessage = "Üst başlık zorunludur.")]
    [StringLength(200)]
    public string Subtitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(6000)]
    public string Description { get; set; } = string.Empty;
}