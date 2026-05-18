using System.ComponentModel.DataAnnotations;
using InventoryTracker.Models;
using Xunit;

namespace InventoryTracker.Tests;

public class UserAccountValidationTests
{
    [Fact]
    public void ValidUserAccount_PassesValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company Inc",
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(account);

        Assert.Empty(results);
    }

    [Fact]
    public void MissingAccountName_FailsValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "",
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(account);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserAccount.AccountName)));
    }

    [Fact]
    public void AccountNameTooShort_FailsValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Short",
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(account);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserAccount.AccountName)));
    }

    [Fact]
    public void AccountNameTooLong_FailsValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = new string('A', 76),
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(account);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserAccount.AccountName)));
    }

    [Fact]
    public void InvalidEmailAddress_FailsValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company Inc",
            AppUserId = "user123",
            AccountEmail = "not-an-email",
            AccountRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(account);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserAccount.AccountEmail)));
    }

    [Fact]
    public void EmailTooShort_FailsValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company Inc",
            AppUserId = "user123",
            AccountEmail = "a@b.c",
            AccountRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(account);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserAccount.AccountEmail)));
    }

    [Fact]
    public void EmailTooLong_FailsValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company Inc",
            AppUserId = "user123",
            AccountEmail = "thisisaverylongemailaddressthatshouldexceedthemaximumlengthoftysixtyfivercharactersapprox@example.com",
            AccountRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(account);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserAccount.AccountEmail)));
    }

    [Fact]
    public void MissingAppUserId_FailsValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company Inc",
            AppUserId = "",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        List<ValidationResult> results = Validate(account);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserAccount.AppUserId)));
    }

    [Fact]
    public void WholesalerRole_PassesValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Wholesaler",
            AppUserId = "user123",
            AccountEmail = "wholesaler@example.com",
            AccountRole = UserRole.Wholesaler
        };

        List<ValidationResult> results = Validate(account);

        Assert.Empty(results);
    }

    [Fact]
    public void AdminRole_PassesValidation()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Admin Account",
            AppUserId = "admin123",
            AccountEmail = "admin@example.com",
            AccountRole = UserRole.Admin
        };

        List<ValidationResult> results = Validate(account);

        Assert.Empty(results);
    }

    [Fact]
    public void ValidAccountHasEmptyProductsCollection()
    {
        UserAccount account = new UserAccount
        {
            UserAccountId = 1,
            AccountName = "Valid Company Inc",
            AppUserId = "user123",
            AccountEmail = "company@example.com",
            AccountRole = UserRole.Manufacturer
        };

        Assert.NotNull(account.Products);
        Assert.Empty(account.Products);
    }

    private static List<ValidationResult> Validate(UserAccount account)
    {
        ValidationContext context = new ValidationContext(account);
        List<ValidationResult> results = new List<ValidationResult>();
        Validator.TryValidateObject(account, context, results, validateAllProperties: true);
        return results;
    }
}
