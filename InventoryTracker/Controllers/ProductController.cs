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
			.Where(p => p.ProductId == id)
			.FirstOrDefaultAsync();

		return View(product);
	}


}
