using System.ComponentModel.DataAnnotations;

namespace InventoryTracker.Models;

public class AddProductViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The product title must be between 3 and 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number.")]
    public decimal Price { get; set; }

    // Future product fields can be added here without changing the page layout.
    public string? Description { get; set; }

    // Reserved for future inventory metadata.
    public string? Sku { get; set; }

    // Reserved for future categorization or vendor-specific options.
    public string? Category { get; set; }
}