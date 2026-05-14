using System.ComponentModel.DataAnnotations;
using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class LoginViewModelValidationTests
{
    [Fact]
    public void ValidModel_PassesValidation()
    {
        LoginViewModel model = new LoginViewModel
        {
            Email = "user@example.com",
            Password = "Valid123!",
            RememberMe = true
        };

        List<ValidationResult> results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void MissingEmail_FailsValidation()
    {
        LoginViewModel model = new LoginViewModel
        {
            Email = string.Empty,
            Password = "Valid123!"
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(LoginViewModel.Email)));
    }

    [Fact]
    public void MissingPassword_FailsValidation()
    {
        LoginViewModel model = new LoginViewModel
        {
            Email = "user@example.com",
            Password = string.Empty
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(LoginViewModel.Password)));
    }

    private static List<ValidationResult> Validate(LoginViewModel model)
    {
        ValidationContext context = new ValidationContext(model);
        List<ValidationResult> results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
