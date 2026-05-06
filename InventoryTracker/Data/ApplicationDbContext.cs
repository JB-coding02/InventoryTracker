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
	/// This allows the DbContext to manage the UserAccounts table in the database,
	/// which stores unified manufacturer and wholesaler accounts.
	/// </summary>
	public DbSet<UserAccount> UserAccounts { get; set; }

	/// <summary>
	/// This allows the DbContext to manage the Product table in the database, 
	/// which shows all products that belong to a UserAccount.
	/// </summary>
	public DbSet<Product> Products { get; set; }

	/// <summary>
	/// This allows the DbContext to manage the Order table in the database,
	/// which stores all orders between Wholesalers and Manufacturers.
	/// </summary>
	public DbSet<Order> Orders { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<UserAccount>()
			.HasOne(u => u.AppUser)
			.WithOne(user => user.UserAccountProfile)
			.HasForeignKey<UserAccount>(u => u.AppUserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<Product>()
			.HasOne(p => p.UserAccount)
			.WithMany(u => u.Products)
			.HasForeignKey(p => p.UserAccountId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<Order>()
			.HasOne(o => o.Wholesaler)
			.WithMany()
			.HasForeignKey(o => o.WholesalerId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<Order>()
			.HasOne(o => o.Manufacturer)
			.WithMany()
			.HasForeignKey(o => o.ManufacturerId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<Order>()
			.HasOne(o => o.Product)
			.WithMany()
			.HasForeignKey(o => o.ProductId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
