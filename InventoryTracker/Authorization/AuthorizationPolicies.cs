namespace InventoryTracker.Authorization;

public static class AuthorizationPolicies
{
    public const string ViewOrders = "ViewOrders";

    public const string ViewManufacturerInventory = "ViewManufacturerInventory";

    public const string AddProduct = "AddProduct";

    public const string ViewAllProducts = "ViewAllProducts";
}