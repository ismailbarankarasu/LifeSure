using System.ComponentModel.DataAnnotations;

namespace LifeSure.Mediator.ContactMessages.Models;

public class ContactMessageInput
{
    [Display(Name = "Contact.FullName")]
    [Required(ErrorMessage = "Validation.Required")]
    [StringLength(150, ErrorMessage = "Validation.MaxLength")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Contact.Email")]
    [Required(ErrorMessage = "Validation.Required")]
    [EmailAddress(ErrorMessage = "Validation.Email")]
    [StringLength(254, ErrorMessage = "Validation.MaxLength")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Contact.Phone")]
    [StringLength(30, ErrorMessage = "Validation.MaxLength")]
    [Phone(ErrorMessage = "Validation.Phone")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Contact.Subject")]
    [Required(ErrorMessage = "Validation.Required")]
    [StringLength(200, ErrorMessage = "Validation.MaxLength")]
    public string Subject { get; set; } = string.Empty;

    [Display(Name = "Contact.Message")]
    [Required(ErrorMessage = "Validation.Required")]
    [StringLength(6000, ErrorMessage = "Validation.MaxLength")]
    public string Message { get; set; } = string.Empty;
}