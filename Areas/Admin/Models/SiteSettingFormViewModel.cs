using System.ComponentModel.DataAnnotations;

namespace LifeSure.Areas.Admin.Models;

public class SiteSettingFormViewModel : IValidatableObject
{
    [Display(Name = "Site adı")]
    [Required(ErrorMessage = "Site adı zorunludur.")]
    [StringLength(150)]
    public string SiteName { get; set; } = "LifeSure";

    [Display(Name = "E-posta adresi")]
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefon numarası")]
    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Adres")]
    [Required(ErrorMessage = "Adres zorunludur.")]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Harita bağlantısı")]
    [StringLength(2000)]
    [Url(ErrorMessage = "Geçerli bir harita bağlantısı giriniz.")]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? MapUrl { get; set; }

    [Display(Name = "Facebook")]
    [StringLength(1000)]
    [Url]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? FacebookUrl { get; set; }

    [Display(Name = "Instagram")]
    [StringLength(1000)]
    [Url]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? InstagramUrl { get; set; }

    [Display(Name = "LinkedIn")]
    [StringLength(1000)]
    [Url]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? LinkedInUrl { get; set; }

    [Display(Name = "X")]
    [StringLength(1000)]
    [Url]
    [RegularExpression(
        @"^https://[^\s\\]+$",
        ErrorMessage = "Bağlantı https:// ile başlamalıdır.")]
    public string? XUrl { get; set; }

    [Display(Name = "Footer açıklaması")]
    [Required(ErrorMessage = "Türkçe footer açıklaması zorunludur.")]
    [StringLength(2000)]
    public string TurkishFooterDescription { get; set; } = string.Empty;

    [Display(Name = "Meta açıklaması")]
    [Required(ErrorMessage = "Türkçe meta açıklaması zorunludur.")]
    [StringLength(500)]
    public string TurkishMetaDescription { get; set; } = string.Empty;

    [Display(Name = "Footer açıklaması")]
    [StringLength(2000)]
    public string? EnglishFooterDescription { get; set; }

    [Display(Name = "Meta açıklaması")]
    [StringLength(500)]
    public string? EnglishMetaDescription { get; set; }

    public bool HasEnglishContent()
    {
        return !string.IsNullOrWhiteSpace(EnglishFooterDescription)
            || !string.IsNullOrWhiteSpace(EnglishMetaDescription);
    }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (!HasEnglishContent())
        {
            yield break;
        }

        if (string.IsNullOrWhiteSpace(EnglishFooterDescription))
        {
            yield return new ValidationResult(
                "İngilizce footer açıklamasını doldurunuz.",
                [nameof(EnglishFooterDescription)]);
        }

        if (string.IsNullOrWhiteSpace(EnglishMetaDescription))
        {
            yield return new ValidationResult(
                "İngilizce meta açıklamasını doldurunuz.",
                [nameof(EnglishMetaDescription)]);
        }
    }
}