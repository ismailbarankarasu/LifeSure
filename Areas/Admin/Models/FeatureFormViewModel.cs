using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeSure.Areas.Admin.Models;

public class FeatureFormViewModel
{
    [BindNever]
    public int Id { get; set; }

    [Required(ErrorMessage = "İkon sınıfı zorunludur.")]
    [StringLength(100)]
    [RegularExpression(
        @"^(fas|far|fab) fa-[a-z0-9-]+$",
        ErrorMessage = "Örnek ikon sınıfı: fas fa-shield-alt")]
    [Display(Name = "İkon Sınıfı")]
    public string IconClass { get; set; } = "fas fa-shield-alt";

    [Range(0, int.MaxValue)]
    [Display(Name = "Gösterim Sırası")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Yayında")]
    public bool IsActive { get; set; } = true;

    [Required]
    [MinLength(1)]
    [MaxLength(2)]
    public List<FeatureTranslationFormModel> Translations { get; set; } =
    [
        new() { LanguageCode = "tr" },
        new() { LanguageCode = "en" }
    ];
}

public class FeatureTranslationFormModel
{
    [Required]
    [RegularExpression(@"^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    public bool HasContent()
    {
        return !string.IsNullOrWhiteSpace(Title)
            || !string.IsNullOrWhiteSpace(Description);
    }
}