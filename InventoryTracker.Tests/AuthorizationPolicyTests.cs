using System.Reflection;
using InventoryTracker.Authorization;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace InventoryTracker.Tests;

public class AuthorizationPolicyTests
{
    [Fact]
    public void OrdersIndex_UsesDatabaseBackedPolicy()
    {
        AuthorizeAttribute attribute = GetAuthorizeAttribute(typeof(InventoryTracker.Controllers.OrdersController), "Index");

        Assert.Equal(AuthorizationPolicies.ViewOrders, attribute.Policy);
    }

    [Fact]
    public void ProductList_UsesDatabaseBackedPolicy()
    {
        AuthorizeAttribute attribute = GetAuthorizeAttribute(typeof(InventoryTracker.Controllers.ProductController), "List");

        Assert.Equal(AuthorizationPolicies.ViewManufacturerInventory, attribute.Policy);
    }

    [Fact]
    public void ProductAll_UsesDatabaseBackedPolicy()
    {
        AuthorizeAttribute attribute = GetAuthorizeAttribute(typeof(InventoryTracker.Controllers.ProductController), "All");

        Assert.Equal(AuthorizationPolicies.ViewAllProducts, attribute.Policy);
    }

    private static AuthorizeAttribute GetAuthorizeAttribute(Type controllerType, string actionName)
    {
        MethodInfo method = controllerType.GetMethod(actionName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Action '{actionName}' was not found on {controllerType.Name}.");

        return method.GetCustomAttributes<AuthorizeAttribute>(inherit: true).Single();
    }
}