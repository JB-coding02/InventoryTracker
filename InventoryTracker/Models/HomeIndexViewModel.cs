using System;
using System.Collections.Generic;

namespace InventoryTracker.Models;

/// <summary>
/// The pagination result model used for displaying paged data in the UI
/// </summary>
/// <typeparam name="T"> The type of items being paged</typeparam>
public sealed class PagedResult<T>
{
	/// <summary>
	/// The item list for the current page of results
	/// </summary>
	public required IReadOnlyList<T> Items { get; init; }
	/// <summary>
	/// The current page number (1-based index)
	/// </summary>
	public required int Page { get; init; }
	/// <summary>
	/// The number of items to display per page (page size)
	/// </summary>
	public required int PageSize { get; init; }
	/// <summary>
	/// The total number of items across all pages (used for calculating total pages and enabling pagination controls)
	/// </summary>
	public required int TotalCount { get; init; }

	/// <summary>
	/// Calculation of the total number of pages based on the total count and page size
	/// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
	/// <summary>
	/// True if there is a previous page available (current page > 1)
	/// </summary>
    public bool HasPreviousPage => Page > 1;
	/// <summary>
	/// True if there is a next page available (current page < total pages)
	/// </summary>
	public bool HasNextPage => Page < TotalPages;
}

/// <summary>
/// The view model representing a summary of a manufacturer account and their associated product count, 
/// used for displaying in the wholesaler dashboard
/// </summary>
public sealed class ManufacturerSummaryViewModel
{
	/// <summary>
	/// The unique manufacturer account identifier to link relevant data to manufacturer dashboard
	/// </summary>
    public int UserAccountId { get; init; }
	/// <summary>
	/// The name of the account associated with the manufacturer
	/// </summary>
    public required string AccountName { get; init; }
	/// <summary>
	/// The email of the associated manufacturer account to be displayed on the wholesaler dashboard
	/// </summary>
    public required string AccountEmail { get; init; }
	/// <summary>
	/// The total count of products associated with the manufacturer account to be displayed on the wholesaler dashboard
	/// </summary>
    public int ProductCount { get; init; }
}

/// <summary>
/// The view model for the home page/dashboard that contains properties to determine the user's authentication status, role,
/// and relevant data to display on the dashboard based on their role
/// </summary>
public sealed class HomeIndexViewModel
{
	/// <summary>
	/// Returns true if the user is currently signed in
	/// </summary>
   public bool IsSignedIn { get; set; }
	/// <summary>
	/// Returns true if the user is in the Manufacturer role
	/// </summary>
    public bool IsManufacturer { get; set; }
	/// <summary>
	/// Returns true if the user is in the Wholesaler role
	/// </summary>
    public bool IsWholesaler { get; set; }
	/// <summary>
	/// The username of the signed in user to be displayed on the dashboard
	/// </summary>

    public string? SignedInAs { get; set; }
	/// <summary>
	/// The unique identifier of the manufacturer user account associated with the signed in user
	/// </summary>
    public int? ManufacturerUserAccountId { get; set; }
	/// <summary>
	/// The name of the manufacturer account associated with the signed in user
	/// </summary>
    public string? ManufacturerAccountName { get; set; }
	/// <summary>
	/// The paged list of products associated with the manufacturer account to be displayed on the manufacturer dashboard
	/// </summary>
    public PagedResult<Product>? ManufacturerProducts { get; set; }
	/// <summary>
	/// A list of manufacturers with products to be displayed on the wholesaler dashboard to make potential orders from
	/// </summary>
    public PagedResult<ManufacturerSummaryViewModel>? ManufacturersWithProducts { get; set; }
}
