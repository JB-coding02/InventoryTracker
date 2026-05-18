@echo off
REM Consolidation to UserAccount - Git Commit Script

cd /d C:\Users\JBurns3236\source\repos\InventoryTracker

echo Staging all changes...
git add -A

echo.
echo Committing changes...
git commit -m "feat: Consolidate ManufacturerAccount and WholesalerAccount into UserAccount

- Created unified UserAccount model to replace separate account types
- Updated ApplicationUser to reference single UserAccountProfile
- Updated Product model to use UserAccountId instead of separate ForeignKeys
- Simplified AccountController registration logic
- Updated ProductController and ProfileController references
- Updated views (Profile, Product, Layout) to use unified UserAccount
- Added comprehensive data migration with no data loss
- All Manufacturer/Wholesaler data migrated to UserAccount with AccountRole
- Migration includes rollback support

Changes:
- New: InventoryTracker/Models/UserAccount.cs
- Modified: ApplicationUser, Product, ApplicationDbContext
- Modified: AccountController, ProductController
- Modified: Views/Profile/Index.cshtml, Views/Product/Index.cshtml, Views/Shared/_Layout.cshtml
- New Migration: ConsolidateToUserAccount.cs"

echo.
echo Changes committed successfully!
echo.
git log --oneline -1
pause
