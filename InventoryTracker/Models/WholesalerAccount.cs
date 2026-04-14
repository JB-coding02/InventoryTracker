using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryTracker.Models;
/// <summary>
/// Represents a single Account registered as a Wholesaler in the inventory tracking system.
/// </summary>
[PrimaryKey(nameof(WholesalerId))]
[Index(nameof(WholesalerName), IsUnique = true)]
[Index(nameof(WholesalerEmail), IsUnique = true)]
public class WholesalerAccount
{
    /// <summary>
    /// Unique Identifier for the wholesaler account
    /// </summary>
    [Key]
    [ReadOnly(true)]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int WholesalerId { get; private set; }

    /// <summary>
    /// Client-facing name of the Wholesaler.
    /// </summary>
    [Required]
    [StringLength(75, MinimumLength = 8, ErrorMessage = "Wholesaler name must be between 8 and 75 characters.")]
    public required string WholesalerName { get; set; }

    /// <summary>
    /// Links this wholesaler profile to the Identity user account.
    /// </summary>
    [Required]
    public required string AppUserId { get; set; }

    [ForeignKey(nameof(AppUserId))]
    public ApplicationUser? AppUser { get; set; }

    /// <summary>
    /// Client-facing email address of the Wholesaler.
    /// </summary>
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(65, MinimumLength = 8, ErrorMessage = "Email must be between 8 and 65 characters.")]
    public required string WholesalerEmail { get; set; }
}
