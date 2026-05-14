using System.ComponentModel.DataAnnotations;
using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class EditProfileViewModelValidationTests
{
    [Fact]
    public void ValidModel_PassesValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "ValidUsername",
            Email = "user@example.com",
            PhoneNumber = "1234567890",
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void MissingUserName_FailsValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "",
            Email = "user@example.com",
            PhoneNumber = "1234567890",
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditProfileViewModel.UserName)));
    }

    [Fact]
    public void UserNameTooShort_FailsValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "AB",
            Email = "user@example.com",
            PhoneNumber = "1234567890",
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditProfileViewModel.UserName)));
    }

    [Fact]
    public void UserNameTooLong_FailsValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = new string('A', 257),
            Email = "user@example.com",
            PhoneNumber = "1234567890",
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditProfileViewModel.UserName)));
    }

    [Fact]
    public void MissingEmail_FailsValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "ValidUsername",
            Email = "",
            PhoneNumber = "1234567890",
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditProfileViewModel.Email)));
    }

    [Fact]
    public void InvalidEmail_FailsValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "ValidUsername",
            Email = "not-an-email",
            PhoneNumber = "1234567890",
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditProfileViewModel.Email)));
    }

    [Fact]
    public void InvalidPhoneNumber_FailsValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "ValidUsername",
            Email = "user@example.com",
            PhoneNumber = "not-a-phone",
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditProfileViewModel.PhoneNumber)));
    }

    [Fact]
    public void PhoneNumberTooLong_FailsValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "ValidUsername",
            Email = "user@example.com",
            PhoneNumber = "12345678901234567890123456",
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditProfileViewModel.PhoneNumber)));
    }

    [Fact]
    public void CompanyNameTooLong_FailsValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "ValidUsername",
            Email = "user@example.com",
            PhoneNumber = "1234567890",
            CompanyName = new string('A', 257),
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditProfileViewModel.CompanyName)));
    }

    [Fact]
    public void NullPhoneNumber_PassesValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "ValidUsername",
            Email = "user@example.com",
            PhoneNumber = null,
            CompanyName = "Valid Company",
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void NullCompanyName_PassesValidation()
    {
        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = "ValidUsername",
            Email = "user@example.com",
            PhoneNumber = "1234567890",
            CompanyName = null,
            UserRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(model);

        Assert.Empty(results);
    }

    private static List<ValidationResult> Validate(EditProfileViewModel model)
    {
        ValidationContext context = new ValidationContext(model);
        List<ValidationResult> results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
