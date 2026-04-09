using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryTracker.Models;
/// <summary>
/// Reresents a single Account registered as a Manufacturer in the inventory tracking system.
/// </summary>
[Index(nameof(ManufacturerName), IsUnique = true)]
public class ManufacturerAccount
{
    /// <summary>
    /// Unique Identifier for the manufacturer account
    /// </summary>
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ManufacturerId { get; set; }

    /// <summary>
    /// Client-facing name of the manufacturer.
    /// </summary>
    [StringLength(75, MinimumLength = 8, ErrorMessage = "Manufacturer name must be between 8 and 75 characters.")]
    public required string ManufacturerName { get; set; }

    /// <summary>
    /// The Password chosen by the manufacturer.
    /// </summary>
    [PasswordPropertyText(true)]
    [StringLength(25, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 25 characters.")]
    
    public required string Password { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(65, MinimumLength = 8, ErrorMessage = "Email must be between 8 and 65 characters.")]
    public required string ManufacturerEmail { get; set; }
}
