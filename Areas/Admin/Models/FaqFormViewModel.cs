using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeSure.Areas.Admin.Models;

public class FaqFormViewModel : IValidatableObject
{
    [BindNever]
    public int? Id { get; set; }

    [Display(Name = "Görüntülenme sırası")]
    [Range(0, int.MaxValue,
        ErrorMessage = "Sıra sıfır veya daha büyük olmalıdır.")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Soru")]
    [Required(ErrorMessage = "Türkçe soru zorunludur.")]
    [StringLength(500)]
    public string TurkishQuestion { get; set; } = string.Empty;

    [Display(Name = "Cevap")]
    [Required(ErrorMessage = "Türkçe cevap zorunludur.")]
    [StringLength(6000)]
    public string TurkishAnswer { get; set; } = string.Empty;

    [Display(Name = "Soru")]
    [StringLength(500)]
    public string? EnglishQuestion { get; set; }

    [Display(Name = "Cevap")]
    [StringLength(6000)]
    public string? EnglishAnswer { get; set; }

    public bool HasEnglishContent()
    {
        return !string.IsNullOrWhiteSpace(EnglishQuestion)
            || !string.IsNullOrWhiteSpace(EnglishAnswer);
    }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (!HasEnglishContent())
        {
            yield break;
        }

        if (string.IsNullOrWhiteSpace(EnglishQuestion))
        {
            yield return new ValidationResult(
                "İngilizce soruyu doldurunuz.",
                [nameof(EnglishQuestion)]);
        }

        if (string.IsNullOrWhiteSpace(EnglishAnswer))
        {
            yield return new ValidationResult(
                "İngilizce cevabı doldurunuz.",
                [nameof(EnglishAnswer)]);
        }
    }
}