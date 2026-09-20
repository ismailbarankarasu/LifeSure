using System.ComponentModel.DataAnnotations;

namespace LifeSure.Mediator.Services.Models;

public class ServiceTranslationDto
{
    [Required]
    [RegularExpression(@"^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    [StringLength(6000)]
    public string Description { get; set; } = string.Empty;
}