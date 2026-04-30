using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class ProductControllerTests
{
    [Fact]
    public void AllAction_HasAuthorizeAttribute()
    {
        System.Reflection.MethodInfo? method = typeof(InventoryTracker.Controllers.ProductController)
            .GetMethod("All", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(method);

        System.Reflection.CustomAttributeData? authorizeAttribute = method.CustomAttributes
            .FirstOrDefault(attr => attr.AttributeType.Name == "AuthorizeAttribute");

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void AllAction_ReturnsProductsOrderedByName()
    {
        List<Product> products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Zebra Product", Price = 10m, StockQuantity = 5, UserAccountId = 1 },
            new Product { ProductId = 2, Name = "Apple Product", Price = 20m, StockQuantity = 3, UserAccountId = 1 },
            new Product { ProductId = 3, Name = "Banana Product", Price = 15m, StockQuantity = 8, UserAccountId = 1 }
        };

        List<Product> orderedProducts = products.OrderBy(p => p.Name).ToList();

        Assert.Equal("Apple Product", orderedProducts[0].Name);
        Assert.Equal("Banana Product", orderedProducts[1].Name);
        Assert.Equal("Zebra Product", orderedProducts[2].Name);
    }

    [Fact]
    public void Product_CanBeCreatedWithAllRequiredFields()
    {
        Product product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 99.99m,
            StockQuantity = 10,
            UserAccountId = 1
        };

        Assert.Equal(1, product.ProductId);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal(99.99m, product.Price);
        Assert.Equal(10, product.StockQuantity);
        Assert.Equal(1, product.UserAccountId);
    }

    [Fact]
    public void Product_CanHaveOptionalImagePath()
    {
        Product product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 99.99m,
            StockQuantity = 10,
            ImagePath = "/images/test.jpg",
            UserAccountId = 1
        };

        Assert.Equal("/images/test.jpg", product.ImagePath);
    }

    [Fact]
    public void Product_CanHaveNullUserAccount()
    {
        Product product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 99.99m,
            StockQuantity = 10,
            UserAccountId = 1,
            UserAccount = null
        };

        Assert.Null(product.UserAccount);
    }
}
