using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeSure.Areas.Admin.Models;

public class AboutFormViewModel
{
    [BindNever]
    public int Id { get; set; }

    [BindNever]
    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Görsel")]
    public IFormFile? ImageFile { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Gösterim Sırası")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Yayında")]
    public bool IsActive { get; set; } = true;

    [Required]
    [MinLength(1)]
    [MaxLength(2)]
    public List<AboutTranslationFormModel> Translations { get; set; } =
    [
        new() { LanguageCode = "tr" },
        new() { LanguageCode = "en" }
    ];
}

public class AboutTranslationFormModel
{
    [Required]
    [RegularExpression(@"^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [StringLength(200)]
    public string? Subtitle { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(6000)]
    public string? Description { get; set; }

    public bool HasContent()
    {
        return !string.IsNullOrWhiteSpace(Subtitle)
            || !string.IsNullOrWhiteSpace(Title)
            || !string.IsNullOrWhiteSpace(Description);
    }
}