using InventoryTracker.Data;
using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Controllers;

public class ProductController : Controller
{
	private readonly ApplicationDbContext _context;

	public ProductController (ApplicationDbContext context)
	{
		_context = context;
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
	public async Task<IActionResult> Add ()
	{
		ViewBag.UserAccounts = await _context.UserAccounts.Where(ua => ua.AccountRole.Equals(UserRole.Manufacturer)).ToListAsync();
		return View();
	}

	/// <summary>
	/// Handles the form submission to add a new product.
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> Add (Product product)
	{
		if (ModelState.IsValid)
		{
			_context.Products.Add(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(List));
		}

		ViewBag.UserAccounts = await _context.UserAccounts.Where(ua => ua.AccountRole.Equals(UserRole.Manufacturer)).ToListAsync();
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
