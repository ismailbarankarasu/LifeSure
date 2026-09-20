using System.ComponentModel.DataAnnotations;

namespace LifeSure.CQRS.Sliders.Models;

public class SliderInput
{
    [Required]
    [StringLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9_-]{11}$")]
    public string? VideoId { get; set; }

    [Required]
    [StringLength(1000)]
    public string ButtonUrl { get; set; } = "/#hizmetler";

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<SliderTranslationDto> Translations { get; set; } = [];
}