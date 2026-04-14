using Microsoft.AspNetCore.Identity;

namespace InventoryTracker.Models;

public class ApplicationUser : IdentityUser
{
    public ManufacturerAccount? ManufacturerProfile { get; set; }

    public WholesalerAccount? WholesalerProfile { get; set; }
}