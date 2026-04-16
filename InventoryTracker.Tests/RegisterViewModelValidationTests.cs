using System.ComponentModel.DataAnnotations;
using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class RegisterViewModelValidationTests
{
    [Fact]
    public void ValidModel_PassesValidation()
    {
        var model = new RegisterViewModel
        {
            AccountType = "Manufacturer",
            CompanyName = "Valid Company",
            Email = "valid@company.com",
            Password = "Valid123!",
            ConfirmPassword = "Valid123!"
        };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void InvalidAccountType_FailsValidation()
    {
        var model = new RegisterViewModel
        {
            AccountType = "Retailer",
            CompanyName = "Valid Company",
            Email = "valid@company.com",
            Password = "Valid123!",
            ConfirmPassword = "Valid123!"
        };

        var results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterViewModel.AccountType)));
    }

    [Fact]
    public void MismatchedPasswords_FailsValidation()
    {
        var model = new RegisterViewModel
        {
            AccountType = "Wholesaler",
            CompanyName = "Valid Company",
            Email = "valid@company.com",
            Password = "Valid123!",
            ConfirmPassword = "Different123!"
        };

        var results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterViewModel.ConfirmPassword)));
    }

    private static List<ValidationResult> Validate(RegisterViewModel model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
