using InventoryTracker.Data;
using InventoryTracker.Models;
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
			.Include(p => p.Manufacturer) // Eager load the related Manufacturer data
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
			.Include(p => p.Manufacturer)
			.ToListAsync();

		return View(products);
	}


}
