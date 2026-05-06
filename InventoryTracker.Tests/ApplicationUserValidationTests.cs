using System.ComponentModel.DataAnnotations;
using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class ApplicationUserValidationTests
{
    [Fact]
    public void ValidApplicationUser_PassesValidation()
    {
        ApplicationUser user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com",
            CompanyName = "Test Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(user);

        Assert.Empty(results);
    }

    [Fact]
    public void CompanyNameTooLong_FailsValidation()
    {
        ApplicationUser user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com",
            CompanyName = new string('A', 101),
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(user);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ApplicationUser.CompanyName)));
    }

    [Fact]
    public void NullCompanyName_PassesValidation()
    {
        ApplicationUser user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com",
            CompanyName = null,
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(user);

        Assert.Empty(results);
    }

    [Fact]
    public void DefaultUserRoleIsManufacturer()
    {
        ApplicationUser user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com"
        };

        Assert.Equal(UserRole.Manufacturer, user.UserRole);
    }

    [Fact]
    public void ApplicationUserCanHaveWholesalerRole()
    {
        ApplicationUser user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com",
            UserRole = UserRole.Wholesaler
        };

        Assert.Equal(UserRole.Wholesaler, user.UserRole);
    }

    [Fact]
    public void ApplicationUserCanHaveAdminRole()
    {
        ApplicationUser user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com",
            UserRole = UserRole.Admin
        };

        Assert.Equal(UserRole.Admin, user.UserRole);
    }

    [Fact]
    public void EmptyCompanyName_PassesValidation()
    {
        ApplicationUser user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com",
            CompanyName = string.Empty,
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(user);

        Assert.Empty(results);
    }

    [Fact]
    public void ValidCompanyName_PassesValidation()
    {
        ApplicationUser user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com",
            CompanyName = "Acme Corporation",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(user);

        Assert.Empty(results);
    }

    private static List<ValidationResult> Validate(ApplicationUser user)
    {
        ValidationContext context = new ValidationContext(user);
        List<ValidationResult> results = new List<ValidationResult>();
        Validator.TryValidateObject(user, context, results, validateAllProperties: true);
        return results;
    }
}
