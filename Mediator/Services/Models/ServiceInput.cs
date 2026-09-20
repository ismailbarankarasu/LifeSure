using System.ComponentModel.DataAnnotations;

namespace LifeSure.Mediator.Services.Models;

public class ServiceInput
{
    [Required]
    [StringLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string IconClass { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<ServiceTranslationDto> Translations { get; set; } = [];
}