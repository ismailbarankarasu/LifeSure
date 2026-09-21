using System.ComponentModel.DataAnnotations;
using LifeSure.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeSure.Areas.Admin.Models;

public class StatisticFormViewModel
{
    [BindNever]
    public int? Id { get; set; }

    [Display(Name = "Değer kaynağı")]
    [EnumDataType(typeof(StatisticSource))]
    public StatisticSource Source { get; set; }

    [Display(Name = "Manuel değer")]
    [Range(0, int.MaxValue,
        ErrorMessage = "Değer sıfır veya daha büyük olmalıdır.")]
    public int Value { get; set; }

    [Display(Name = "Sayı sonundaki ek")]
    [StringLength(20,
        ErrorMessage = "Ek en fazla 20 karakter olabilir.")]
    public string? Suffix { get; set; }

    [Display(Name = "Görüntülenme sırası")]
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Türkçe başlık")]
    [Required(ErrorMessage = "Türkçe başlık zorunludur.")]
    [StringLength(200)]
    public string TurkishTitle { get; set; } = string.Empty;

    [Display(Name = "İngilizce başlık")]
    [StringLength(200)]
    public string? EnglishTitle { get; set; }
}