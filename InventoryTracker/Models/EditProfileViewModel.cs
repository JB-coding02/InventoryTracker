using System.ComponentModel.DataAnnotations;

namespace InventoryTracker.Models;

/// <summary>
/// Represents the data model for the user profile editing form.
/// Used to collect and validate user updates to their profile including username, email, contact information, and role.
/// </summary>
public class EditProfileViewModel
{
    /// <summary>
    /// The user's login username.
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters")]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    /// <summary>
    /// The user's contact phone number.
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The name of the company or business associated with this user's account.
    /// </summary>
    [StringLength(256, ErrorMessage = "Company name cannot exceed 256 characters")]
    [Display(Name = "Company Name")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// The user's email address used for authentication and communication.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    /// <summary>
    /// The user's assigned role (Manufacturer, Wholesaler, or Admin) that determines permissions.
    /// </summary>
    [Required(ErrorMessage = "User role is required")]
    [Display(Name = "User Role")]
    public UserRole UserRole { get; set; } = UserRole.Manufacturer;
}
