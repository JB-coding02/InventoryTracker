using Microsoft.AspNetCore.Mvc;

namespace InventoryTracker.Controllers;

public class ProductController : Controller
{
	public IActionResult Index ()
	{
		return View();
	}
}
