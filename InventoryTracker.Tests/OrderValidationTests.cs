using System.ComponentModel.DataAnnotations;
using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class OrderValidationTests
{
    [Fact]
    public void ValidOrder_PassesValidation()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = 10,
            OrderDate = DateTime.Now,
            Status = "Pending"
        };

        List<ValidationResult> results = Validate(order);

        Assert.Empty(results);
    }

    [Fact]
    public void ZeroQuantity_FailsValidation()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = 0,
            OrderDate = DateTime.Now,
            Status = "Pending"
        };

        List<ValidationResult> results = Validate(order);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Order.Quantity)));
    }

    [Fact]
    public void NegativeQuantity_FailsValidation()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = -5,
            OrderDate = DateTime.Now,
            Status = "Pending"
        };

        List<ValidationResult> results = Validate(order);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Order.Quantity)));
    }

    [Fact]
    public void OrderWithValidStatus_PassesValidation()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = 10,
            OrderDate = DateTime.Now,
            Status = "Delivered"
        };

        List<ValidationResult> results = Validate(order);

        Assert.Empty(results);
    }

    [Fact]
    public void OrderWithLongStatus_PassesValidation()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = 10,
            OrderDate = DateTime.Now,
            Status = "In Transit - Delayed"
        };

        List<ValidationResult> results = Validate(order);

        Assert.Empty(results);
    }

    [Fact]
    public void OrderWithPastDate_PassesValidation()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = 10,
            OrderDate = DateTime.Now.AddDays(-5),
            Status = "Pending"
        };

        List<ValidationResult> results = Validate(order);

        Assert.Empty(results);
    }

    [Fact]
    public void OrderWithFutureDate_PassesValidation()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = 10,
            OrderDate = DateTime.Now.AddDays(5),
            Status = "Pending"
        };

        List<ValidationResult> results = Validate(order);

        Assert.Empty(results);
    }

    [Fact]
    public void LargeQuantity_PassesValidation()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = 1000000,
            OrderDate = DateTime.Now,
            Status = "Pending"
        };

        List<ValidationResult> results = Validate(order);

        Assert.Empty(results);
    }

    [Fact]
    public void OrderCanHaveNullNavigationProperties()
    {
        Order order = new Order
        {
            OrderId = 1,
            WholesalerId = 1,
            ManufacturerId = 2,
            ProductId = 1,
            Quantity = 10,
            OrderDate = DateTime.Now,
            Status = "Pending",
            Wholesaler = null,
            Manufacturer = null,
            Product = null
        };

        Assert.Null(order.Wholesaler);
        Assert.Null(order.Manufacturer);
        Assert.Null(order.Product);
    }

    private static List<ValidationResult> Validate(Order order)
    {
        ValidationContext context = new ValidationContext(order);
        List<ValidationResult> results = new List<ValidationResult>();
        Validator.TryValidateObject(order, context, results, validateAllProperties: true);
        return results;
    }
}
