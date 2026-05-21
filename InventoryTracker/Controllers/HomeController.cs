using InventoryTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventoryTracker.Controllers
{
    /// <summary>
    /// Manages the public-facing pages of the application including the homepage and privacy information.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Displays the application homepage.
        /// </summary>
        /// <returns>The home index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the application privacy policy.
        /// </summary>
        /// <returns>The privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays an error page with the current request trace ID for diagnostic purposes.
        /// </summary>
        /// <returns>The error view with the current request ID and trace information.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
