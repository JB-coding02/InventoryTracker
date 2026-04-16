using System.ComponentModel.DataAnnotations;
using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class LoginViewModelValidationTests
{
    [Fact]
    public void ValidModel_PassesValidation()
    {
        var model = new LoginViewModel
        {
            Email = "user@example.com",
            Password = "Valid123!",
            RememberMe = true
        };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void MissingEmail_FailsValidation()
    {
        var model = new LoginViewModel
        {
            Email = string.Empty,
            Password = "Valid123!"
        };

        var results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(LoginViewModel.Email)));
    }

    [Fact]
    public void MissingPassword_FailsValidation()
    {
        var model = new LoginViewModel
        {
            Email = "user@example.com",
            Password = string.Empty
        };

        var results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(LoginViewModel.Password)));
    }

    private static List<ValidationResult> Validate(LoginViewModel model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
