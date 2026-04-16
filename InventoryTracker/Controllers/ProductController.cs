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


}
