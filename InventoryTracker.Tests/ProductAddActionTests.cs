using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace InventoryTracker.Tests;

public class ProductAddActionTests
{
    [Fact]
    public void AddAction_IsProtectedByManufacturerInventoryPolicy()
    {
        MethodInfo method = typeof(InventoryTracker.Controllers.ProductController)
            .GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, [typeof(InventoryTracker.Models.AddProductViewModel)])
            ?? throw new InvalidOperationException("Add action was not found.");

        AuthorizeAttribute? authorizeAttribute = method.GetCustomAttributes<AuthorizeAttribute>(inherit: true).SingleOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("ViewManufacturerInventory", authorizeAttribute!.Policy);
    }
}