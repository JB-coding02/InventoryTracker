namespace InventoryTracker.Models;

public class Product
{

    /// <summary>
    /// The unique primary key identifier for the product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The price of the product in US dollars.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The quantity of the item that is currently in stock.
    /// </summary>
    public int StockQuantity { get; set; }
}
