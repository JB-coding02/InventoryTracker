using InventoryTracker.Data;
using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Controllers;

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
	public async Task<IActionResult> List ()
	{
		List<Product> products = await _context.Products
			.Include(p => p.UserAccount)
			.ToListAsync();

		return View(products);
	}
	/// <summary>
	/// Displays the form to add a new product.
	/// </summary>
	[HttpGet]
 [Authorize(Roles = nameof(UserRole.Manufacturer))]
	public async Task<IActionResult> Add ()
	{
		// Ensure the user is authenticated and has a manufacturer account before allowing access to the add product form.
		ApplicationUser? user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return Challenge();
		}

		UserAccount? userAccount = await _context.UserAccounts
			.AsNoTracking()
			.FirstOrDefaultAsync(a => a.AppUserId == user.Id && a.AccountRole == UserRole.Manufacturer);

		if (userAccount == null)
		{
			return Forbid();
		}

		ViewBag.UserAccounts = new List<UserAccount> { userAccount };
		return View();
	}

	/// <summary>
	/// Handles the form submission to add a new product.
	/// </summary>
	[HttpPost]
  [Authorize(Roles = nameof(UserRole.Manufacturer))]
	public async Task<IActionResult> Add (Product product)
	{
		// Ensure the user is authenticated and has a manufacturer account before allowing product creation.
		ApplicationUser? user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return Challenge();
		}

		// Verify the user has a manufacturer account.
		UserAccount? userAccount = await _context.UserAccounts
			.AsNoTracking()
			.FirstOrDefaultAsync(a => a.AppUserId == user.Id && a.AccountRole == UserRole.Manufacturer);

		if (userAccount == null)
		{
			return Forbid();
		}

		// Force ownership to the current manufacturer account.
		product.UserAccountId = userAccount.UserAccountId;

		if (ModelState.IsValid)
		{
			_context.Products.Add(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(List));
		}

     ViewBag.UserAccounts = new List<UserAccount> { userAccount };
		return View(product);
	}

	/// <summary>
	/// Displays all products in the system (Admin only).
	/// </summary>
	/// <returns>The task result contains an <see cref="IActionResult"/> that
	/// renders the all products view with a list of all products.</returns>
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> All()
	{
		List<Product> allProducts = await _context.Products
			.Include(p => p.UserAccount) // Eager load the related UserAccount data
			.OrderBy(p => p.Name)
			.ToListAsync();


		return View(allProducts);
	}


}
