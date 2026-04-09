using InventoryTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {

        /// <summary>
        /// This allows the DbContext to manage the Product table in the database, 
        /// which shows all products that belong to a Manufacturer.
        /// </summary>
        public DbSet<Product> Products { get; set; }
    }
}
