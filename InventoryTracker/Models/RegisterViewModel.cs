using System.ComponentModel.DataAnnotations;

namespace InventoryTracker.Models;

public class RegisterViewModel
{
    /// <summary>
    /// Selected account type used to determine whether the profile is created
    /// in <c>ManufacturerAccounts</c> or <c>WholesalerAccounts</c>.
    /// </summary>
    [Required]
    [RegularExpression("Manufacturer|Wholesaler", ErrorMessage = "Please select a valid account type.")]
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// Company name for the registering organization.
    /// </summary>
    [Required]
    [StringLength(75, MinimumLength = 8)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Company contact email used as the login identity.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(65, MinimumLength = 8)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Plain-text password input validated by Identity and persisted as a hash.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation that must match <see cref="Password"/>.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string PasswordRequirements => "Password must be at least 6 characters and meet the configured Identity password rules.";
}
