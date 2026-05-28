using InventoryTracker.Data;
using InventoryTracker.Authorization;
using InventoryTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Controllers;

/// <summary>
/// Manages product-related operations including viewing individual product details and listing products in the inventory system.
/// Provides public product browsing and administrative views for all products.
/// </summary>
public class ProductController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;

	public ProductController (ApplicationDbContext context, UserManager<ApplicationUser> userManager)
	{
		_context = context;
		_userManager = userManager;
	}


	/// <summary>
	/// Displays the details view for the specified product.
	/// </summary>
	/// <param name="id">The unique identifier of the product to display.</param>
	/// <returns>The task result contains an <see cref="IActionResult"/> that
	/// renders the product details view if the product is found; otherwise, a NotFound result.</returns>

	public async Task<IActionResult> Index (int id)
	{
		Product? product = await _context.Products
			.Include(p => p.UserAccount) // Eager load the related UserAccount data
			.FirstOrDefaultAsync(p => p.ProductId == id);

		if (product == null) { 
			return NotFound();
		}
		return View(product);
	}
	/// <summary>
	/// Displays a listing of all products with their manufacturer info.
	/// </summary>
	/// <returns>A view containing all products.</returns>
	[Authorize(Policy = AuthorizationPolicies.ViewManufacturerInventory)]
	public async Task<IActionResult> List ()
	{
		List<Product> products = await _context.Products
			.Include(p => p.UserAccount)
			.ToListAsync();

		return View(products);
	}

	[HttpGet]
	[Authorize(Policy = AuthorizationPolicies.ViewManufacturerInventory)]
	public IActionResult Add()
	{
		return View(new AddProductViewModel());
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policy = AuthorizationPolicies.ViewManufacturerInventory)]
	public async Task<IActionResult> Add(AddProductViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
		if (currentUser == null)
		{
			return Unauthorized();
		}

		UserAccount? manufacturerAccount = await _context.UserAccounts
			.FirstOrDefaultAsync(account => account.AppUserId == currentUser.Id);
		if (manufacturerAccount == null)
		{
			ModelState.AddModelError(string.Empty, "Your account profile could not be found.");
			return View(model);
		}

		Product product = new Product
		{
			Name = model.Title,
			Price = model.Price,
			StockQuantity = 0,
			UserAccountId = manufacturerAccount.UserAccountId
		};

		_context.Products.Add(product);
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(List));
	}

	/// <summary>
	/// Displays all products in the system (Admin only).
	/// </summary>
	/// <returns>The task result contains an <see cref="IActionResult"/> that
	/// renders the all products view with a list of all products.</returns>
	[Authorize(Policy = AuthorizationPolicies.ViewAllProducts)]
	public async Task<IActionResult> All()
	{
		List<Product> allProducts = await _context.Products
			.Include(p => p.UserAccount) // Eager load the related UserAccount data
			.OrderBy(p => p.Name)
			.ToListAsync();

		return View(allProducts);
	}


}
