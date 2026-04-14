using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryTracker.Models;

/// <summary>
/// Represents a single product that belongs to a Manufacturer in the inventory tracking system.
/// </summary>
[PrimaryKey(nameof(ProductId))]
public class Product
{

	/// <summary>
	/// The unique primary key identifier for the product.
	/// </summary>
	[Key]
	[ReadOnly(true)]
    public int ProductId { get; set; }

	/// <summary>
	/// The name of the product.
	/// </summary>
	[Required]
	[StringLength(100, MinimumLength = 3, ErrorMessage = "The product name must be between 3 and 100 characters.")]
	public required string Name { get; set; }

	/// <summary>
	/// The price of the product in US dollars.
	/// Stored with two decimal places.
	/// </summary>
	[Range(0, int.MaxValue)]
	[Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

	/// <summary>
	/// The quantity of the item that is currently in stock.
	/// </summary>
	[Required]
	[Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative number.")]
	public int StockQuantity { get; set; }

	/// <summary>
	/// Relationship to the ManufacturerAccount that produces this product.
	/// </summary>
	[Required]
	public int ManufacturerId { get; set; }

	/// <summary>
	/// Navigation property to the ManufacturerAccount that produces this product.
	/// </summary>
	[ForeignKey(nameof(ManufacturerId))]
	public ManufacturerAccount? Manufacturer { get; set; }
}
