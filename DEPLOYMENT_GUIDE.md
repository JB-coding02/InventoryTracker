# UserAccount Consolidation - Deployment Guide

## Pre-Deployment Checklist

- [ ] Backup production database
- [ ] Review all changes in `CONSOLIDATION_SUMMARY.md`
- [ ] Test in development environment
- [ ] Verify no active user sessions during migration
- [ ] Ensure database write access
- [ ] Have rollback plan ready

## Deployment Steps

### Option 1: Entity Framework Code-First Migration (Recommended)

```powershell
# Navigate to project directory
cd C:\Users\JBurns3236\source\repos\InventoryTracker\InventoryTracker

# Apply migration to database
dotnet ef database update

# Verify migration applied successfully
# Check database for:
# - UserAccounts table exists with all data
# - Products.UserAccountId populated correctly
# - ManufacturerAccounts and WholesalerAccounts tables removed
```

### Option 2: Manual SQL Script

If needed, extract the SQL from the migration:

```powershell
# Generate SQL script from migration
dotnet ef migrations script -o consolidation.sql

# Review consolidation.sql before execution
# Execute against target database
```

## Post-Deployment Verification

### Database Checks

```sql
-- Verify UserAccounts table
SELECT COUNT(*) as TotalAccounts,
       SUM(CASE WHEN AccountRole = 0 THEN 1 ELSE 0 END) as ManufacturerCount,
       SUM(CASE WHEN AccountRole = 1 THEN 1 ELSE 0 END) as WholesalerCount
FROM UserAccounts;

-- Verify Product mappings
SELECT COUNT(*) as ProductsWithAccount
FROM Products
WHERE UserAccountId IS NOT NULL;

-- Verify old tables are removed
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('ManufacturerAccounts', 'WholesalerAccounts');
```

### Application Checks

1. **Login Test**
   - Create new Manufacturer account
   - Verify profile displays correctly
   - Verify navigation shows "My Inventory"

2. **Login Test**
   - Create new Wholesaler account
   - Verify profile displays correctly
   - Verify navigation shows "My Orders"

3. **Product Test**
   - View existing product
   - Verify account information displays
   - Verify no broken links

4. **Profile Test**
   - Edit profile
   - Verify role-based content displays
   - Verify changes persist

## Rollback Procedure

If issues occur:

```powershell
# Rollback to previous migration
dotnet ef database update <PreviousMigrationName>

# Example (replace with actual migration name):
# dotnet ef database update AddIdentityProfileExtensions
```

The migration includes a full `Down()` method that will:
- Recreate `ManufacturerAccounts` table with all original data
- Recreate `WholesalerAccounts` table with all original data
- Restore Product foreign key relationships
- Remove `UserAccounts` table

## Troubleshooting

### Issue: Migration fails - "Violation of UNIQUE constraint"

**Solution:**
- Ensure no duplicate emails or names in accounts
- Check if products exist without accounts
- Manually clean conflicting data before retry

### Issue: Products have NULL UserAccountId after migration

**Solution:**
- Check Products table for orphaned records (no manufacturer/wholesaler)
- Update orphaned products with correct account manually
- Set default account if business rules allow

### Issue: LoginFailed for ApplicationUser

**Solution:**
- Verify NavigationProperty loaded correctly
- Check DbContext is properly updated
- Clear any cached references in application

## Support Contact

For migration issues or questions:
1. Review changes in `CONSOLIDATION_SUMMARY.md`
2. Check database logs for SQL errors
3. Verify Entity Framework version compatibility

## Version Information

- .NET Version: 10
- Entity Framework: Latest (from project file)
- Migration Name: ConsolidateToUserAccount
- Created: 2024
