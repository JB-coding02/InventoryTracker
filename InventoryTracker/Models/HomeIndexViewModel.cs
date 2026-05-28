using System;
using System.Collections.Generic;

namespace InventoryTracker.Models;

public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public sealed class ManufacturerSummaryViewModel
{
    public int UserAccountId { get; init; }
    public required string AccountName { get; init; }
    public required string AccountEmail { get; init; }
    public int ProductCount { get; init; }
}

public sealed class HomeIndexViewModel
{
   public bool IsSignedIn { get; set; }
    public bool IsManufacturer { get; set; }
    public bool IsWholesaler { get; set; }

    public string? SignedInAs { get; set; }

    public int? ManufacturerUserAccountId { get; set; }
    public string? ManufacturerAccountName { get; set; }

    public PagedResult<Product>? ManufacturerProducts { get; set; }
    public PagedResult<ManufacturerSummaryViewModel>? ManufacturersWithProducts { get; set; }
}
