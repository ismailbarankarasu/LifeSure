using System.ComponentModel.DataAnnotations;

namespace LifeSure.CQRS.Features.Models;

public class FeatureTranslationDto
{
    [Required]
    [RegularExpression(@"^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required(ErrorMessage = "Özellik başlığı zorunludur.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Özellik açıklaması zorunludur.")]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
}