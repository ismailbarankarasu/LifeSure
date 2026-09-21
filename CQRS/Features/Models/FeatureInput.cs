using System.ComponentModel.DataAnnotations;

namespace LifeSure.CQRS.Features.Models;

public class FeatureInput
{
    [Required]
    [StringLength(100)]
    [RegularExpression(
        @"^(fas|far|fab) fa-[a-z0-9-]+$",
        ErrorMessage = "İkon sınıfını 'fas fa-shield-alt' biçiminde giriniz.")]
    public string IconClass { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<FeatureTranslationDto> Translations { get; set; } = [];
}