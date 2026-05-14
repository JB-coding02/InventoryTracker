using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryTracker.Models;

/// <summary>
/// Represents an order between a Wholesaler and a Manufacturer in the inventory tracking system.
/// </summary>
public class Order
{
    /// <summary>
    /// Unique identifier for the order.
    /// </summary>
    [Key]
    [ReadOnly(true)]
    public int OrderId { get; set; }

    /// <summary>
    /// The UserAccount of the Wholesaler placing the order.
    /// </summary>
    [Required]
    public int WholesalerId { get; set; }

    [ForeignKey(nameof(WholesalerId))]
    public UserAccount? Wholesaler { get; set; }

    /// <summary>
    /// The UserAccount of the Manufacturer fulfilling the order.
    /// </summary>
    [Required]
    public int ManufacturerId { get; set; }

    [ForeignKey(nameof(ManufacturerId))]
    public UserAccount? Manufacturer { get; set; }

    /// <summary>
    /// The Product being ordered.
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    /// <summary>
    /// The quantity of the product being ordered.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    /// <summary>
    /// The order date and time.
    /// </summary>
    [Required]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// The status of the order (e.g., Pending, Shipped, Delivered, Cancelled).
    /// </summary>
    [Required]
    [StringLength(50)]
    public required string Status { get; set; }
}
