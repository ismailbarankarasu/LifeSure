using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeSure.Areas.Admin.Models;

public class ServiceFormViewModel : IValidatableObject
{
    [BindNever]
    public int? Id { get; set; }

    [BindNever]
    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Hizmet görseli")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "İkon sınıfı")]
    [Required(ErrorMessage = "İkon sınıfı zorunludur.")]
    [StringLength(100)]
    [RegularExpression(
        @"^(fas|far|fab) fa-[a-z0-9-]+$",
        ErrorMessage = "Örnek ikon formatı: fas fa-heart")]
    public string IconClass { get; set; } = "fas fa-shield-alt";

    [Display(Name = "Görüntülenme sırası")]
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Başlık")]
    [Required(ErrorMessage = "Türkçe başlık zorunludur.")]
    [StringLength(200)]
    public string TurkishTitle { get; set; } = string.Empty;

    [Display(Name = "Kısa açıklama")]
    [Required(ErrorMessage = "Türkçe kısa açıklama zorunludur.")]
    [StringLength(500)]
    public string TurkishShortDescription { get; set; } = string.Empty;

    [Display(Name = "Tam açıklama")]
    [Required(ErrorMessage = "Türkçe tam açıklama zorunludur.")]
    [StringLength(6000)]
    public string TurkishDescription { get; set; } = string.Empty;

    [Display(Name = "Başlık")]
    [StringLength(200)]
    public string? EnglishTitle { get; set; }

    [Display(Name = "Kısa açıklama")]
    [StringLength(500)]
    public string? EnglishShortDescription { get; set; }

    [Display(Name = "Tam açıklama")]
    [StringLength(6000)]
    public string? EnglishDescription { get; set; }

    public bool HasEnglishContent()
    {
        return !string.IsNullOrWhiteSpace(EnglishTitle)
            || !string.IsNullOrWhiteSpace(EnglishShortDescription)
            || !string.IsNullOrWhiteSpace(EnglishDescription);
    }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (!HasEnglishContent())
        {
            yield break;
        }

        if (string.IsNullOrWhiteSpace(EnglishTitle))
        {
            yield return new ValidationResult(
                "İngilizce başlığı doldurunuz.",
                [nameof(EnglishTitle)]);
        }

        if (string.IsNullOrWhiteSpace(EnglishShortDescription))
        {
            yield return new ValidationResult(
                "İngilizce kısa açıklamayı doldurunuz.",
                [nameof(EnglishShortDescription)]);
        }

        if (string.IsNullOrWhiteSpace(EnglishDescription))
        {
            yield return new ValidationResult(
                "İngilizce tam açıklamayı doldurunuz.",
                [nameof(EnglishDescription)]);
        }
    }
}