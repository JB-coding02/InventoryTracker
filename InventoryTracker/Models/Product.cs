using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryTracker.Models;

public class Product
{

    /// <summary>
    /// The unique primary key identifier for the product.
    /// </summary>
    [Key]
    public int ProductId { get; set; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The price of the product in US dollars.
    /// Stored with two decimal places.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// The quantity of the item that is currently in stock.
    /// </summary>
    public int StockQuantity { get; set; }

	/// <summary>
	/// Relationship to the ManufacturerAccount that produces this product.
	/// </summary>
	[Required]
	public int ManufacturerId { get; set; }

	[ForeignKey(nameof(ManufacturerId))]
	public ManufacturerAccount? Manufacturer { get; set; }
}
