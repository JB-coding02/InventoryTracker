using InventoryTracker.Data;
using InventoryTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InventoryTracker.Controllers;

public class HomeController : Controller
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = DefaultPageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = DefaultPageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

		// Initialize the view model with basic user information
		var vm = new HomeIndexViewModel
        {
            IsSignedIn = User.Identity?.IsAuthenticated == true,
            SignedInAs = User.Identity?.Name
        };

        if (!vm.IsSignedIn)
        {
            return View(vm);
        }

		// Determine the user's roles to customize the view accordingly
		vm.IsManufacturer = User.IsInRole(nameof(UserRole.Manufacturer));
        vm.IsWholesaler = User.IsInRole(nameof(UserRole.Wholesaler));

        ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return View(vm);
        }

		// Load the user's account information to display relevant data
		UserAccount? currentAccount = await _db.UserAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AppUserId == currentUser.Id);

		vm.SignedInAs = currentAccount?.AccountName ?? currentUser.UserName;

		if (vm.IsManufacturer)
        {
			// If the user is a manufacturer, load their products to display on the dashboard
			if (currentAccount != null)
            {
                vm.ManufacturerUserAccountId = currentAccount.UserAccountId;
                vm.ManufacturerAccountName = currentAccount.AccountName;
            }

			// Query the products associated with the current manufacturer account, applying pagination
			IQueryable<Product> query = _db.Products
                .AsNoTracking()
                .Include(p => p.UserAccount);

			// Ensure that only products belonging to the current manufacturer are included in the results
			if (currentAccount != null)
            {
                query = query.Where(p => p.UserAccountId == currentAccount.UserAccountId);
            }
            else
            {
                query = query.Where(p => false);
            }

			// Get the total count of products for pagination purposes
			int totalCount = await query.CountAsync();
            List<Product> items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

			// Populate the view model with the paginated list of products for the manufacturer dashboard
			vm.ManufacturerProducts = new PagedResult<Product>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        else if (vm.IsWholesaler)
        {
			// If the user is a wholesaler, load a summary of manufacturers and their product counts to display on the dashboard
			IQueryable<ManufacturerSummaryViewModel> query = _db.UserAccounts
                .AsNoTracking()
                .Where(a => a.AccountRole == UserRole.Manufacturer)
                .Select(a => new ManufacturerSummaryViewModel
                {
                    UserAccountId = a.UserAccountId,
                    AccountName = a.AccountName,
                    AccountEmail = a.AccountEmail,
                    ProductCount = a.Products.Count
                })
                .Where(m => m.ProductCount > 0);

			// Get the total count of manufacturers with products
			int totalCount = await query.CountAsync();
            List<ManufacturerSummaryViewModel> items = await query
                .OrderBy(m => m.AccountName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

			// Populate the view model with the paginated list of manufacturers and their product counts for the wholesaler dashboard
			vm.ManufacturersWithProducts = new PagedResult<ManufacturerSummaryViewModel>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
