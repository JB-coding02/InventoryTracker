using InventoryTracker.Data;
using InventoryTracker.Authorization;
using InventoryTracker.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => 
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.ViewOrders, policy =>
        policy.Requirements.Add(new UserRoleRequirement(UserRole.Admin, UserRole.Wholesaler)));

    options.AddPolicy(AuthorizationPolicies.ViewManufacturerInventory, policy =>
        policy.Requirements.Add(new UserRoleRequirement(UserRole.Manufacturer)));

    options.AddPolicy(AuthorizationPolicies.ViewAllProducts, policy =>
        policy.Requirements.Add(new UserRoleRequirement(UserRole.Admin)));
});
builder.Services.AddScoped<IAuthorizationHandler, UserRoleAuthorizationHandler>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

// Apply migrations in development only; production should use explicit deployment migration steps.
if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        IEnumerable<string> pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
    catch (SqlException ex) when (ex.Number == 2714)
    {
        app.Logger.LogWarning(ex, "Skipping automatic migration because a database object already exists.");
    }
}

#if DEBUG 
	// Seed account roles on startup
	await RoleSeedService.SeedRolesAsync(app.Services);
	// Seed admin account on startup
	await RoleSeedService.SeedAdminAccountAsync(app.Services);
#endif


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapRazorPages();

app.Run();
