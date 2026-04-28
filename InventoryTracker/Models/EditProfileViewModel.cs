using System.ComponentModel.DataAnnotations;

namespace InventoryTracker.Models;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters")]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
}
