using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryTracker.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string? CompanyName { get; set; }

    public ManufacturerAccount? ManufacturerProfile { get; set; }

    public WholesalerAccount? WholesalerProfile { get; set; }
}