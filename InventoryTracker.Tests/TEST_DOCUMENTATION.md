## Unit Tests Added to InventoryTracker

This document summarizes all unit tests added to the InventoryTracker project to improve code coverage and validation.

### Test Files Created

#### 1. **ProductValidationTests.cs** (9 tests)
Tests for the `Product` model validation rules:
- `ValidProduct_PassesValidation` - Validates a correctly formatted product
- `ProductWithoutName_FailsValidation` - Ensures product name is required
- `ProductNameTooShort_FailsValidation` - Validates minimum name length (3 chars)
- `ProductNameTooLong_FailsValidation` - Validates maximum name length (100 chars)
- `NegativePrice_FailsValidation` - Prevents negative prices
- `NegativeStockQuantity_FailsValidation` - Prevents negative stock quantities
- `ZeroStockQuantity_PassesValidation` - Allows zero stock
- `ProductWithImagePath_PassesValidation` - Validates optional image path
- `ZeroPrice_PassesValidation` - Allows zero price

#### 2. **UserAccountValidationTests.cs** (10 tests)
Tests for the `UserAccount` model validation rules:
- `ValidUserAccount_PassesValidation` - Validates a correctly formatted account
- `MissingAccountName_FailsValidation` - Ensures account name is required
- `AccountNameTooShort_FailsValidation` - Validates minimum name length (8 chars)
- `AccountNameTooLong_FailsValidation` - Validates maximum name length (75 chars)
- `InvalidEmailAddress_FailsValidation` - Validates email format
- `EmailTooShort_FailsValidation` - Validates minimum email length (6 chars)
- `EmailTooLong_FailsValidation` - Validates maximum email length (65 chars)
- `MissingAppUserId_FailsValidation` - Ensures AppUserId is required
- `WholesalerRole_PassesValidation` - Validates Wholesaler role assignment
- `AdminRole_PassesValidation` - Validates Admin role assignment
- `ValidAccountHasEmptyProductsCollection` - Ensures initial Products collection is empty

#### 3. **EditProfileViewModelValidationTests.cs** (11 tests)
Tests for the `EditProfileViewModel` model validation rules:
- `ValidModel_PassesValidation` - Validates a correctly formatted profile update
- `MissingUserName_FailsValidation` - Ensures username is required
- `UserNameTooShort_FailsValidation` - Validates minimum username length (3 chars)
- `UserNameTooLong_FailsValidation` - Validates maximum username length (256 chars)
- `MissingEmail_FailsValidation` - Ensures email is required
- `InvalidEmail_FailsValidation` - Validates email format
- `InvalidPhoneNumber_FailsValidation` - Validates phone format when provided
- `PhoneNumberTooLong_FailsValidation` - Validates maximum phone length (20 chars)
- `CompanyNameTooLong_FailsValidation` - Validates maximum company name length (256 chars)
- `NullPhoneNumber_PassesValidation` - Allows null phone number
- `NullCompanyName_PassesValidation` - Allows null company name

#### 4. **ErrorViewModelTests.cs** (6 tests)
Tests for the `ErrorViewModel` model:
- `ErrorViewModelWithRequestId_ShowRequestIdReturnsTrue` - Shows request ID when present
- `ErrorViewModelWithNullRequestId_ShowRequestIdReturnsFalse` - Hides request ID when null
- `ErrorViewModelWithEmptyRequestId_ShowRequestIdReturnsFalse` - Hides request ID when empty
- `ErrorViewModelWithWhitespaceRequestId_ShowRequestIdReturnsTrue` - Shows whitespace-only request ID
- `ErrorViewModelDefaultConstructor_CreatesValidInstance` - Validates default construction
- `ErrorViewModelRequestIdCanBeSet` - Validates property modification

#### 5. **ApplicationUserValidationTests.cs** (8 tests)
Tests for the `ApplicationUser` model validation rules:
- `ValidApplicationUser_PassesValidation` - Validates a correctly formatted user
- `CompanyNameTooLong_FailsValidation` - Validates maximum company name length (100 chars)
- `NullCompanyName_PassesValidation` - Allows null company name
- `DefaultUserRoleIsManufacturer` - Validates default role is Manufacturer
- `ApplicationUserCanHaveWholesalerRole` - Validates Wholesaler role assignment
- `ApplicationUserCanHaveAdminRole` - Validates Admin role assignment
- `EmptyCompanyName_PassesValidation` - Allows empty company name
- `ValidCompanyName_PassesValidation` - Validates valid company name

#### 6. **Updated Existing Tests**
Modified the following test files to use explicit types (instead of `var`):
- `LoginViewModelValidationTests.cs` - 3 tests for login validation
- `RegisterViewModelValidationTests.cs` - 3 tests for registration validation

### Test Statistics

- **Total Tests: 51**
- **All Passing: ✓**
- **Coverage Areas:**
  - Model Validation (data annotations)
  - Boundary Testing (min/max lengths, ranges)
  - Optional Property Handling
  - Role Assignment and Defaults
  - State and Property Behavior

### Running Tests

Execute all tests using:
```powershell
dotnet test InventoryTracker.Tests
```

Or in Visual Studio:
- Open Test Explorer (Test → Test Explorer)
- Select "InventoryTracker.Tests"
- Click "Run All Tests"

### Test Methodology

All validation tests use `System.ComponentModel.DataAnnotations.Validator.TryValidateObject()` to validate model constraints defined via data annotation attributes. This ensures tests are robust and maintainable as requirements change.
