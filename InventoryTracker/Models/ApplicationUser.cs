using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryTracker.Models;

/// <summary>
/// Represents an application user account that extends the ASP.NET Core Identity IdentityUser class.
/// Contains additional profile information and role management for users in the inventory tracking system.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// The company or business name associated with this user's account.
    /// </summary>
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// The role assigned to this user (Manufacturer, Wholesaler, or Admin).
    /// Determines the user's permissions and access level within the application.
    /// </summary>
    [Required]
    public UserRole UserRole { get; set; } = UserRole.Manufacturer;

    /// <summary>
    /// Navigation property that links this user to their associated user account profile.
    /// </summary>
    public UserAccount? UserAccountProfile { get; set; }
}