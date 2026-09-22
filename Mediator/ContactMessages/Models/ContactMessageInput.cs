using System.ComponentModel.DataAnnotations;

namespace LifeSure.Mediator.ContactMessages.Models;

public class ContactMessageInput
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Konu zorunludur.")]
    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesaj zorunludur.")]
    [StringLength(6000)]
    public string Message { get; set; } = string.Empty;
}