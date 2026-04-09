using InventoryTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

	/// <summary>
	/// This allows the DbContext to manage the
	/// ManufacturerAccounts table in the database,
	/// which stores all registered manufacturer accounts.
	/// </summary>
    public DbSet<ManufacturerAccount> ManufacturerAccounts { get; set; }

	/// <summary>
	/// This allows the DbContext to manage the WholesalerAccounts table
	/// in the database, which stores all registered wholesaler accounts.
	/// </summary>
    public DbSet<WholesalerAccount> WholesalerAccounts { get; set; }
}
