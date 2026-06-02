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
public class ProductController (ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
{
	private readonly ApplicationDbContext _context = context;
	private readonly UserManager<ApplicationUser> _userManager = userManager;


	/// <summary>
	/// Displays the details view for the specified product.
	/// </summary>
	/// <param name="id">The unique identifier of the product to display.</param>
	/// <returns>The task result contains an <see cref="IActionResult"/> that
	/// renders the product details view if the product is found; otherwise, a NotFound result.</returns>

	public async Task<IActionResult> Index (int id)
	{
		Product? product = await _context.Products
			.AsNoTracking()
			.Include(p => p.UserAccount) // Eager load the related UserAccount data
			.FirstOrDefaultAsync(p => p.ProductId == id);

		if (product == null) { 
			return NotFound();
		}

		ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
		string backToListAction = nameof(List);
		bool canEditOrDelete = false;

		if (currentUser != null)
		{
			if (currentUser.UserRole == UserRole.Admin)
			{
				backToListAction = nameof(All);
				canEditOrDelete = true;
			}
			else if (currentUser.UserRole == UserRole.Manufacturer)
			{
				backToListAction = nameof(List);
				UserAccount? manufacturerAccount = await _context.UserAccounts
					.AsNoTracking()
					.FirstOrDefaultAsync(ua => ua.AppUserId == currentUser.Id);
				if (manufacturerAccount != null && product.UserAccountId == manufacturerAccount.UserAccountId)
				{
					canEditOrDelete = true;
				}
			}
		}

		ViewBag.BackToListAction = backToListAction;
		ViewBag.CanEditOrDelete = canEditOrDelete;

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
			.AsNoTracking()
			.Include(p => p.UserAccount)
			.ToListAsync();
		return View(products);
	}
	/// <summary>
	/// Displays the form to add a new product.
	/// </summary>
	[HttpGet]
	[Authorize(Policy = AuthorizationPolicies.AddProduct)]
	public IActionResult Add ()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policy = AuthorizationPolicies.AddProduct)]
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
			.AsNoTracking()
			.FirstOrDefaultAsync(account => account.AppUserId == currentUser.Id);
		if (manufacturerAccount == null)
		{
			ModelState.AddModelError(string.Empty, "Your account profile could not be found.");
			return View(model);
		}

		Product product = new()
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
			.AsNoTracking()
			.Include(p => p.UserAccount) // Eager load the related UserAccount data
			.OrderBy(p => p.Name)
			.ToListAsync();


		return View(allProducts);
	}

	/// <summary>
	/// Displays the edit product form pre-populated with the current product's information.
	/// </summary>
	[HttpGet]
	[Authorize]
	public async Task<IActionResult> Edit (int id)
	{
		ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
		if (currentUser == null)
		{
			return Unauthorized();
		}

		if (currentUser.UserRole != UserRole.Admin && currentUser.UserRole != UserRole.Manufacturer)
		{
			return Forbid();
		}

		Product? product = await _context.Products
			.AsNoTracking()
			.Include(p => p.UserAccount)
			.FirstOrDefaultAsync(p => p.ProductId == id);
		if (product == null)
		{
			return NotFound();
		}

		if (currentUser.UserRole == UserRole.Manufacturer)
		{
			UserAccount? manufacturerAccount = await _context.UserAccounts
				.AsNoTracking()
				.FirstOrDefaultAsync(ua => ua.AppUserId == currentUser.Id);
			if (manufacturerAccount == null || product.UserAccountId != manufacturerAccount.UserAccountId)
			{
				return Forbid();
			}
		}
		else if (currentUser.UserRole == UserRole.Admin)
		{
			ViewBag.UserAccounts = await _context.UserAccounts
				.AsNoTracking()
				.Where(ua => ua.AccountRole == UserRole.Manufacturer)
				.OrderBy(ua => ua.AccountName)
				.ToListAsync();
		}

		return View(product);
	}

	/// <summary>
	/// Handles form submission to save changes to an existing product.
	/// </summary>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize]
	public async Task<IActionResult> Edit (int id, Product product)
	{
		if (id != product.ProductId)
		{
			return NotFound();
		}

		ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
		if (currentUser == null)
		{
			return Unauthorized();
		}

		if (currentUser.UserRole != UserRole.Admin && currentUser.UserRole != UserRole.Manufacturer)
		{
			return Forbid();
		}

		Product? existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
		if (existingProduct == null)
		{
			return NotFound();
		}

		if (currentUser.UserRole == UserRole.Manufacturer)
		{
			UserAccount? manufacturerAccount = await _context.UserAccounts
				.AsNoTracking()
				.FirstOrDefaultAsync(ua => ua.AppUserId == currentUser.Id);
			if (manufacturerAccount == null || existingProduct.UserAccountId != manufacturerAccount.UserAccountId)
			{
				return Forbid();
			}
			
			// A manufacturer cannot reassign a product to a different manufacturer
			product.UserAccountId = existingProduct.UserAccountId;
		}

		if (ModelState.IsValid)
		{
			try
			{
				_context.Update(product);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await ProductExists(product.ProductId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return currentUser.UserRole == UserRole.Admin 
				? RedirectToAction(nameof(All)) 
				: RedirectToAction(nameof(List));
		}

		if (currentUser.UserRole == UserRole.Admin)
		{
			ViewBag.UserAccounts = await _context.UserAccounts
				.AsNoTracking()
				.Where(ua => ua.AccountRole == UserRole.Manufacturer)
				.OrderBy(ua => ua.AccountName)
				.ToListAsync();
		}

		return View(product);
	}

	/// <summary>
	/// Handles deletion of a product.
	/// </summary>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize]
	public async Task<IActionResult> Delete (int id)
	{
		ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
		if (currentUser == null)
		{
			return Unauthorized();
		}

		if (currentUser.UserRole != UserRole.Admin && currentUser.UserRole != UserRole.Manufacturer)
		{
			return Forbid();
		}

		Product? product = await _context.Products.FindAsync(id);
		if (product == null)
		{
			return NotFound();
		}

		if (currentUser.UserRole == UserRole.Manufacturer)
		{
			UserAccount? manufacturerAccount = await _context.UserAccounts
				.AsNoTracking()
				.FirstOrDefaultAsync(ua => ua.AppUserId == currentUser.Id);
			if (manufacturerAccount == null || product.UserAccountId != manufacturerAccount.UserAccountId)
			{
				return Forbid();
			}
		}

		_context.Products.Remove(product);
		await _context.SaveChangesAsync();

		return currentUser.UserRole == UserRole.Admin 
			? RedirectToAction(nameof(All)) 
			: RedirectToAction(nameof(List));
	}

	private async Task<bool> ProductExists(int id)
	{
		return await _context.Products.AnyAsync(e => e.ProductId == id);
	}
}
