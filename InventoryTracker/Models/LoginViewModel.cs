using System.ComponentModel.DataAnnotations;

namespace InventoryTracker.Models;

public class LoginViewModel
{
    /// <summary>
    /// User email used as the sign-in identifier.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password submitted for authentication.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the authentication cookie should persist across browser sessions.
    /// </summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
