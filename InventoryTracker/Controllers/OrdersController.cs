using InventoryTracker.Data;
using InventoryTracker.Authorization;
using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Controllers;

/// <summary>
/// Manages order-related operations including viewing and filtering orders in the system.
/// This controller provides administrative functionality to view all orders with search and filtering capabilities.
/// </summary>
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays all orders with search and filter capabilities for admins and wholesalers.
    /// </summary>
    /// <param name="searchTerm">Search term to filter orders by product name or ID.</param>
    /// <param name="wholesalerId">Filter orders by Wholesaler ID.</param>
    /// <param name="manufacturerId">Filter orders by Manufacturer ID.</param>
    /// <returns>Returns the All Orders view with filtered results.</returns>
    [Authorize(Policy = AuthorizationPolicies.ViewOrders)]
    public async Task<IActionResult> Index(string? searchTerm, string? wholesalerId, string? manufacturerId)
    {
        IQueryable<Order> ordersQuery = _context.Orders
            .Include(o => o.Wholesaler)
            .Include(o => o.Manufacturer)
            .Include(o => o.Product);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            int? orderId = null;
            if (int.TryParse(searchTerm, out int parsedId))
            {
                orderId = parsedId;
            }

            ordersQuery = ordersQuery.Where(o =>
                (orderId.HasValue && o.OrderId == orderId.Value) ||
                o.Product!.Name.Contains(searchTerm) ||
                o.Status.Contains(searchTerm));
        }

        // Apply Wholesaler filter
        if (!string.IsNullOrWhiteSpace(wholesalerId))
        {
            ordersQuery = ordersQuery.Where(o => o.WholesalerId == wholesalerId);
        }

        // Apply Manufacturer filter
        if (!string.IsNullOrWhiteSpace(manufacturerId))
        {
            ordersQuery = ordersQuery.Where(o => o.ManufacturerId == manufacturerId);
        }

        // Execute query and order by date
        List<Order> filteredOrders = await ordersQuery
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        // Get lists for dropdowns - ApplicationUsers with Wholesaler role
        List<ApplicationUser> wholesalers = await _context.Users
            .Where(u => u.UserRole == UserRole.Wholesaler)
            .OrderBy(u => u.CompanyName)
            .ToListAsync();

        // Get lists for dropdowns - ApplicationUsers with Manufacturer role
        List<ApplicationUser> manufacturers = await _context.Users
            .Where(u => u.UserRole == UserRole.Manufacturer)
            .OrderBy(u => u.CompanyName)
            .ToListAsync();

        // Pass data to view
        ViewData["SearchTerm"] = searchTerm;
        ViewData["SelectedWholesalerId"] = wholesalerId;
        ViewData["SelectedManufacturerId"] = manufacturerId;
        ViewData["Wholesalers"] = wholesalers;
        ViewData["Manufacturers"] = manufacturers;

        return View(filteredOrders);
    }

    /// <summary>
    /// Creates a new order for the current wholesaler with the specified product and quantity.
    /// </summary>
    /// <param name="productId">The ID of the product to order.</param>
    /// <param name="quantity">The quantity of the product to order.</param>
    /// <returns>Redirects to the Orders Index view on success; otherwise, redirects back with error message.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> CreateOrder(int productId, int quantity)
    {
        // Verify user is authenticated and is a wholesaler
        ApplicationUser? currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        if (currentUser.UserRole != UserRole.Wholesaler)
        {
            return Forbid();
        }

        // Validate quantity
        if (quantity < 1)
        {
            TempData["ErrorMessage"] = "Quantity must be at least 1.";
            return RedirectToAction("List", "Product");
        }

        // Get the product with its manufacturer information
        Product? product = await _context.Products
            .Include(p => p.UserAccount)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
        {
            TempData["ErrorMessage"] = "Product not found.";
            return RedirectToAction("List", "Product");
        }

        // Get the manufacturer's ApplicationUser ID
        if (product.UserAccount == null)
        {
            TempData["ErrorMessage"] = "Product manufacturer not found.";
            return RedirectToAction("List", "Product");
        }

        ApplicationUser? manufacturer = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == product.UserAccount.AppUserId);

        if (manufacturer == null)
        {
            TempData["ErrorMessage"] = "Product manufacturer user not found.";
            return RedirectToAction("List", "Product");
        }

        // Create the order
        Order order = new()
        {
            WholesalerId = currentUser.Id,
            ManufacturerId = manufacturer.Id,
            ProductId = productId,
            Quantity = quantity,
            OrderDate = DateTime.Now,
            Status = "Pending"
        };

        try
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Order created successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error creating order: {ex.Message}";
        }

        return RedirectToAction("Index");
    }
}
