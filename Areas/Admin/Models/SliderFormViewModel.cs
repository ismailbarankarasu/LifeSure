using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeSure.Areas.Admin.Models;

public class SliderFormViewModel
{
    [BindNever]
    public int Id { get; set; }

    [BindNever]
    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Görsel")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "YouTube Video Kimliği")]
    [RegularExpression(
        @"^[a-zA-Z0-9_-]{11}$",
        ErrorMessage = "Video kimliği 11 karakter olmalıdır.")]
    public string? VideoId { get; set; }

    [Required(ErrorMessage = "Buton bağlantısı zorunludur.")]
    [StringLength(1000)]
    [Display(Name = "Buton Bağlantısı")]
    public string ButtonUrl { get; set; } = "/#hizmetler";

    [Range(0, int.MaxValue)]
    [Display(Name = "Gösterim Sırası")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Yayında")]
    public bool IsActive { get; set; } = true;

    [Required]
    [MinLength(1)]
    [MaxLength(2)]
    public List<SliderTranslationFormModel> Translations { get; set; } =
    [
        new() { LanguageCode = "tr" },
        new() { LanguageCode = "en" }
    ];
}

public class SliderTranslationFormModel
{
    [Required]
    [RegularExpression(@"^(tr|en)$")]
    public string LanguageCode { get; set; } = "tr";

    [StringLength(200)]
    public string? Subtitle { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? ButtonText { get; set; }

    public bool HasContent()
    {
        return !string.IsNullOrWhiteSpace(Subtitle)
            || !string.IsNullOrWhiteSpace(Title)
            || !string.IsNullOrWhiteSpace(Description)
            || !string.IsNullOrWhiteSpace(ButtonText);
    }
}