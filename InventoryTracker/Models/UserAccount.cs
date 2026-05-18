using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryTracker.Models;

/// <summary>
/// Represents a unified account for both Manufacturers and Wholesalers in the inventory tracking system.
/// </summary>
[PrimaryKey(nameof(UserAccountId))]
[Index(nameof(AccountName), IsUnique = true)]
[Index(nameof(AccountEmail), IsUnique = true)]
public class UserAccount
{
    /// <summary>
    /// Unique Identifier for the user account
    /// </summary>
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserAccountId { get; set; }

    /// <summary>
    /// Client-facing name of the account (company/business name).
    /// </summary>
    [Required]
    [StringLength(75, MinimumLength = 8, ErrorMessage = "Account name must be between 8 and 75 characters.")]
    public required string AccountName { get; set; }

    /// <summary>
    /// Links this account profile to the Identity user account.
    /// </summary>
    [Required]
    public required string AppUserId { get; set; }

    [ForeignKey(nameof(AppUserId))]
    public ApplicationUser? AppUser { get; set; }

    /// <summary>
    /// The email address of the account to contact.
    /// </summary>
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(65, MinimumLength = 6, ErrorMessage = "Email must be between 6 and 65 characters.")]
    public required string AccountEmail { get; set; }

    /// <summary>
    /// The role of this account (Manufacturer, Wholesaler, or Admin).
    /// Provides type information for the account.
    /// </summary>
    [Required]
    public required UserRole AccountRole { get; set; }

    /// <summary>
    /// The collection of products associated with this account.
    /// A single account can have many products.
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
