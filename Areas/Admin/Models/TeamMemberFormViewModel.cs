using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeSure.Areas.Admin.Models;

public class TeamMemberFormViewModel
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

    [Display(Name = "Türkçe görev")]
    [Required(ErrorMessage = "Türkçe görev bilgisi zorunludur.")]
    [StringLength(150)]
    public string TurkishJobTitle { get; set; } = string.Empty;

    [Display(Name = "İngilizce görev")]
    [StringLength(150)]
    public string? EnglishJobTitle { get; set; }

    [Display(Name = "Facebook bağlantısı")]
    [StringLength(1000)]
    [Url(ErrorMessage = "Geçerli bir Facebook bağlantısı giriniz.")]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? FacebookUrl { get; set; }

    [Display(Name = "Instagram bağlantısı")]
    [StringLength(1000)]
    [Url(ErrorMessage = "Geçerli bir Instagram bağlantısı giriniz.")]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? InstagramUrl { get; set; }

    [Display(Name = "LinkedIn bağlantısı")]
    [StringLength(1000)]
    [Url(ErrorMessage = "Geçerli bir LinkedIn bağlantısı giriniz.")]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? LinkedInUrl { get; set; }

    [Display(Name = "X bağlantısı")]
    [StringLength(1000)]
    [Url(ErrorMessage = "Geçerli bir X bağlantısı giriniz.")]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? XUrl { get; set; }

    [Display(Name = "Görüntülenme sırası")]
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
}