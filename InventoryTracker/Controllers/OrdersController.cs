using InventoryTracker.Data;
using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Controllers;

public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays all orders with search and filter capabilities (admin only).
    /// </summary>
    /// <param name="searchTerm">Search term to filter orders by product name or ID.</param>
    /// <param name="wholesalerId">Filter orders by Wholesaler ID.</param>
    /// <param name="manufacturerId">Filter orders by Manufacturer ID.</param>
    /// <returns>Returns the All Orders view with filtered results.</returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string? searchTerm, int? wholesalerId, int? manufacturerId)
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
        if (wholesalerId.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.WholesalerId == wholesalerId.Value);
        }

        // Apply Manufacturer filter
        if (manufacturerId.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.ManufacturerId == manufacturerId.Value);
        }

        // Execute query and order by date
        List<Order> filteredOrders = await ordersQuery
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        // Get lists for dropdowns
        List<UserAccount> wholesalers = await _context.UserAccounts
            .Where(u => u.AccountRole == UserRole.Wholesaler)
            .OrderBy(u => u.AccountName)
            .ToListAsync();

        List<UserAccount> manufacturers = await _context.UserAccounts
            .Where(u => u.AccountRole == UserRole.Manufacturer)
            .OrderBy(u => u.AccountName)
            .ToListAsync();

        // Pass data to view
        ViewData["SearchTerm"] = searchTerm;
        ViewData["SelectedWholesalerId"] = wholesalerId;
        ViewData["SelectedManufacturerId"] = manufacturerId;
        ViewData["Wholesalers"] = wholesalers;
        ViewData["Manufacturers"] = manufacturers;

        return View(filteredOrders);
    }
}
