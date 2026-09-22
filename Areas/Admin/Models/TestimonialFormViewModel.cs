using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeSure.Areas.Admin.Models;

public class TestimonialFormViewModel : IValidatableObject
{
    [BindNever]
    public int? Id { get; set; }

    [BindNever]
    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Ad soyad")]
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Fotoğraf")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Puan")]
    [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
    public int Rating { get; set; } = 5;

    [Display(Name = "Görüntülenme sırası")]
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Unvan")]
    [Required(ErrorMessage = "Türkçe unvan zorunludur.")]
    [StringLength(150)]
    public string TurkishTitle { get; set; } = string.Empty;

    [Display(Name = "Yorum")]
    [Required(ErrorMessage = "Türkçe yorum zorunludur.")]
    [StringLength(3000)]
    public string TurkishComment { get; set; } = string.Empty;

    [Display(Name = "Unvan")]
    [StringLength(150)]
    public string? EnglishTitle { get; set; }

    [Display(Name = "Yorum")]
    [StringLength(3000)]
    public string? EnglishComment { get; set; }

    public bool HasEnglishContent()
    {
        return !string.IsNullOrWhiteSpace(EnglishTitle)
            || !string.IsNullOrWhiteSpace(EnglishComment);
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
                "İngilizce unvanı doldurunuz.",
                [nameof(EnglishTitle)]);
        }

        if (string.IsNullOrWhiteSpace(EnglishComment))
        {
            yield return new ValidationResult(
                "İngilizce yorumu doldurunuz.",
                [nameof(EnglishComment)]);
        }
    }
}