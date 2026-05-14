using InventoryTracker.Data;
using InventoryTracker.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

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
        bool canConnect = await dbContext.Database.CanConnectAsync();
        if (!canConnect)
        {
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            DbConnection connection = dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            try
            {
                using DbCommand tableCountCommand = connection.CreateCommand();
                tableCountCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                int tableCount = Convert.ToInt32(await tableCountCommand.ExecuteScalarAsync());

                using DbCommand historyCommand = connection.CreateCommand();
                historyCommand.CommandText = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory'";
                bool historyExists = await historyCommand.ExecuteScalarAsync() != null;

                if (tableCount == 0 || historyExists)
                {
                    await dbContext.Database.MigrateAsync();
                }
                else
                {
                    app.Logger.LogWarning("Skipping automatic migration because tables already exist without migration history.");
                }
            }
            finally
            {
                await connection.CloseAsync();
            }
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
