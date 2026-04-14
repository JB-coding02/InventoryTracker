using InventoryTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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

	/// <summary>
	/// This allows the DbContext to manage the Product table in the database, 
	/// which shows all products that belong to a Manufacturer.
	/// </summary>
	public DbSet<Product> Products { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<ManufacturerAccount>()
			.HasOne(m => m.AppUser)
			.WithOne(u => u.ManufacturerProfile)
			.HasForeignKey<ManufacturerAccount>(m => m.AppUserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<WholesalerAccount>()
			.HasOne(w => w.AppUser)
			.WithOne(u => u.WholesalerProfile)
			.HasForeignKey<WholesalerAccount>(w => w.AppUserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
