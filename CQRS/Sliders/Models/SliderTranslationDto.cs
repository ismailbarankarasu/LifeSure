using System.ComponentModel.DataAnnotations;

namespace LifeSure.CQRS.Sliders.Models;

public class SliderTranslationDto
{
    [Required]
    [RegularExpression(@"^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required]
    [StringLength(200)]
    public string Subtitle { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ButtonText { get; set; } = string.Empty;
}