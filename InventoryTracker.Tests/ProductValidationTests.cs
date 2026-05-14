using System.ComponentModel.DataAnnotations;
using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class ProductValidationTests
{
    [Fact]
    public void ValidProduct_PassesValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company",
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        Product product = new Product
        {
            ProductId = 1,
            Name = "Valid Product",
            Price = 99.99m,
            StockQuantity = 10,
            UserAccountId = 1,
            UserAccount = account
        };

        List<ValidationResult> results = Validate(product);

        Assert.Empty(results);
    }

    [Fact]
    public void ProductWithoutName_FailsValidation()
    {
        Product product = new Product
        {
            ProductId = 1,
            Name = "",
            Price = 99.99m,
            StockQuantity = 10,
            UserAccountId = 1
        };

        List<ValidationResult> results = Validate(product);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.Name)));
    }

    [Fact]
    public void ProductNameTooShort_FailsValidation()
    {
        Product product = new Product
        {
            ProductId = 1,
            Name = "AB",
            Price = 99.99m,
            StockQuantity = 10,
            UserAccountId = 1
        };

        List<ValidationResult> results = Validate(product);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.Name)));
    }

    [Fact]
    public void ProductNameTooLong_FailsValidation()
    {
        Product product = new Product
        {
            ProductId = 1,
            Name = new string('A', 101),
            Price = 99.99m,
            StockQuantity = 10,
            UserAccountId = 1
        };

        List<ValidationResult> results = Validate(product);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.Name)));
    }

    [Fact]
    public void NegativePrice_FailsValidation()
    {
        Product product = new Product
        {
            ProductId = 1,
            Name = "Valid Product",
            Price = -10m,
            StockQuantity = 10,
            UserAccountId = 1
        };

        List<ValidationResult> results = Validate(product);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.Price)));
    }

    [Fact]
    public void NegativeStockQuantity_FailsValidation()
    {
        Product product = new Product
        {
            ProductId = 1,
            Name = "Valid Product",
            Price = 99.99m,
            StockQuantity = -5,
            UserAccountId = 1
        };

        List<ValidationResult> results = Validate(product);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.StockQuantity)));
    }

    [Fact]
    public void ZeroStockQuantity_PassesValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company",
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        Product product = new Product
        {
            ProductId = 1,
            Name = "Valid Product",
            Price = 99.99m,
            StockQuantity = 0,
            UserAccountId = 1,
            UserAccount = account
        };

        List<ValidationResult> results = Validate(product);

        Assert.Empty(results);
    }

    [Fact]
    public void ProductWithImagePath_PassesValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company",
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        Product product = new Product
        {
            ProductId = 1,
            Name = "Valid Product",
            Price = 99.99m,
            StockQuantity = 10,
            ImagePath = "/images/product.jpg",
            UserAccountId = 1,
            UserAccount = account
        };

        List<ValidationResult> results = Validate(product);

        Assert.Empty(results);
    }

    [Fact]
    public void ZeroPrice_PassesValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company",
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        Product product = new Product
        {
            ProductId = 1,
            Name = "Valid Product",
            Price = 0m,
            StockQuantity = 10,
            UserAccountId = 1,
            UserAccount = account
        };

        List<ValidationResult> results = Validate(product);

        Assert.Empty(results);
    }

    private static List<ValidationResult> Validate(Product product)
    {
        ValidationContext context = new ValidationContext(product);
        List<ValidationResult> results = new List<ValidationResult>();
        Validator.TryValidateObject(product, context, results, validateAllProperties: true);
        return results;
    }
}
