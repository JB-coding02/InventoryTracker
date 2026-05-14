# UserAccount Consolidation - Architecture Overview

## Before Consolidation

```
┌─────────────────────────────────────────────────────────────────┐
│                    ApplicationUser                               │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ - UserName                                              │   │
│  │ - Email                                                 │   │
│  │ - CompanyName                                          │   │
│  │ - UserRole (Enum: Manufacturer, Wholesaler, Admin)     │   │
│  │ - ManufacturerProfile? (one-to-one)                    │   │
│  │ - WholesalerProfile? (one-to-one)                      │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
       │                                      │
       │                                      │
       ▼                                      ▼
┌──────────────────────────┐      ┌──────────────────────────┐
│   ManufacturerAccount    │      │    WholesalerAccount     │
│ ┌────────────────────┐   │      │ ┌────────────────────┐   │
│ │ ManufacturerId (PK)│   │      │ │ WholesalerId (PK)  │   │
│ │ ManufacturerName   │   │      │ │ WholesalerName     │   │
│ │ ManufacturerEmail  │   │      │ │ WholesalerEmail    │   │
│ │ AppUserId (FK)     │   │      │ │ AppUserId (FK)     │   │
│ └────────────────────┘   │      │ └────────────────────┘   │
└──────────────────────────┘      └──────────────────────────┘
       │                                    │
       └────────────────┬───────────────────┘
                        │
                        ▼
                   ┌─────────────┐
                   │   Product   │
                   │ ┌─────────┐ │
                   │ │ProductId│ │
                   │ │Name     │ │
                   │ │Price    │ │
                   │ │Stock    │ │
                   │ │Image    │ │
                   │ │ManId (FK)   │
                   │ │WholesalerId │
                   │ │  (FK/NULL)  │
                   │ └─────────┘ │
                   └─────────────┘

Problems:
✗ Duplicate code and validation
✗ Two separate profiles per user
✗ Duplicate database tables
✗ Multiple foreign keys in Product table
✗ Complex business logic with conditional checks
```

## After Consolidation

```
┌──────────────────────────────────────────────────────────────┐
│                  ApplicationUser                              │
│  ┌──────────────────────────────────────────────────────┐    │
│  │ - UserName                                           │    │
│  │ - Email                                              │    │
│  │ - CompanyName                                        │    │
│  │ - UserRole (Enum: Manufacturer, Wholesaler, Admin)   │    │
│  │ - UserAccountProfile? (one-to-one)                   │    │
│  └──────────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────────┘
                          │
                          │
                          ▼
         ┌────────────────────────────────┐
         │      UserAccount               │
         │  ┌────────────────────────┐   │
         │  │ UserAccountId (PK)     │   │
         │  │ AccountName            │   │
         │  │ AccountEmail           │   │
         │  │ AccountRole            │   │
         │  │   (Manufacturer|        │   │
         │  │    Wholesaler)         │   │
         │  │ AppUserId (FK)         │   │
         │  └────────────────────────┘   │
         └────────────────────────────────┘
                          │
                          │
                          ▼
                   ┌─────────────┐
                   │   Product   │
                   │ ┌─────────┐ │
                   │ │ProductId│ │
                   │ │Name     │ │
                   │ │Price    │ │
                   │ │Stock    │ │
                   │ │Image    │ │
                   │ │UAId (FK)│ │
                   │ └─────────┘ │
                   └─────────────┘

Benefits:
✓ Single model for all account types
✓ Reduced code duplication
✓ Single database table
✓ Simplified Product relationships
✓ Cleaner business logic
✓ Type discrimination via AccountRole enum
✓ Easier to extend for future account types
```

## Entity Relationship Diagram (After Consolidation)

```
┌─────────────────────────┐
│    AspNetUsers          │
│─────────────────────────│
│ Id (PK)                 │
│ UserName                │
│ Email                   │
│ CompanyName             │
│ UserRole                │
└──────────┬──────────────┘
           │ 1
           │
           │ (one-to-one)
           │
           │ N
┌──────────▼──────────────┐
│   UserAccounts          │
│─────────────────────────│
│ UserAccountId (PK)      │
│ AccountName (UNIQUE)    │
│ AccountEmail (UNIQUE)   │
│ AccountRole             │
│ AppUserId (FK)          │
└──────────┬──────────────┘
           │ 1
           │
           │ (one-to-many)
           │
           │ N
┌──────────▼──────────────┐
│   Products              │
│─────────────────────────│
│ ProductId (PK)          │
│ Name                    │
│ Price                   │
│ StockQuantity           │
│ ImagePath               │
│ UserAccountId (FK)      │
└─────────────────────────┘
```

## Data Migration Flow

```
BEFORE:
┌──────────────────────┐      ┌──────────────────────┐
│ ManufacturerAccounts │      │ WholesalerAccounts   │
│──────────────────────│      │──────────────────────│
│ Id=1: Acme Corp      │      │ Id=1: Dist Inc       │
│ Id=2: TechWorks      │      │ Id=2: GlobalTrade    │
└──────────────────────┘      └──────────────────────┘

                        ▼ MIGRATION ▼

AFTER:
┌──────────────────────────────────────────┐
│         UserAccounts                     │
│──────────────────────────────────────────│
│ Id=1: Acme Corp      | Role=Manufacturer │
│ Id=2: TechWorks      | Role=Manufacturer │
│ Id=3: Dist Inc       | Role=Wholesaler   │
│ Id=4: GlobalTrade    | Role=Wholesaler   │
└──────────────────────────────────────────┘

Products mapping:
- Product.ManufacturerId=1 → Product.UserAccountId=1
- Product.ManufacturerId=2 → Product.UserAccountId=2
- Product.WholesalerId=1 → Product.UserAccountId=3
- Product.WholesalerId=2 → Product.UserAccountId=4
```

## Code Example - Before vs After

### Before: Accessing Account Information

```csharp
// Checking if user has account
if (user.ManufacturerProfile != null)
{
    var name = user.ManufacturerProfile.ManufacturerName;
    var email = user.ManufacturerProfile.ManufacturerEmail;
}

if (user.WholesalerProfile != null)
{
    var name = user.WholesalerProfile.WholesalerName;
    var email = user.WholesalerProfile.WholesalerEmail;
}

// Querying products
var products = await _context.Products
    .Include(p => p.Manufacturer)
    .Include(p => p.Wholesaler)
    .ToListAsync();
```

### After: Accessing Account Information

```csharp
// Checking if user has account
if (user.UserAccountProfile != null)
{
    var name = user.UserAccountProfile.AccountName;
    var email = user.UserAccountProfile.AccountEmail;
    var role = user.UserAccountProfile.AccountRole;
}

// Querying products
var products = await _context.Products
    .Include(p => p.UserAccount)
    .ToListAsync();
```

## Registration Flow Comparison

### Before

```
User Registration
    │
    ├─► Create ApplicationUser
    │
    ├─► Add to Role
    │
    ├─► IF Manufacturer
    │   └─► Add ManufacturerAccount
    │
    └─► IF Wholesaler
        └─► Add WholesalerAccount
```

### After

```
User Registration
    │
    ├─► Create ApplicationUser
    │
    ├─► Add to Role
    │
    └─► Add UserAccount
        (Role determined by AccountRole)
```

## Navigation & View Logic

### Before

```
@if (user.ManufacturerProfile != null)
{
    <li><a href="/Inventory">My Inventory</a></li>
}

@if (user.WholesalerProfile != null)
{
    <li><a href="/Orders">My Orders</a></li>
}
```

### After

```
@if (user.UserAccountProfile?.AccountRole == UserRole.Manufacturer)
{
    <li><a href="/Inventory">My Inventory</a></li>
}

@if (user.UserAccountProfile?.AccountRole == UserRole.Wholesaler)
{
    <li><a href="/Orders">My Orders</a></li>
}
```

## Impact Summary

| Aspect | Before | After |
|--------|--------|-------|
| **Tables** | 3 (Manufacturers, Wholesalers, Products) | 2 (UserAccounts, Products) |
| **Profiles per User** | 2 possible (Mfg or Wholesaler) | 1 (UserAccount with role) |
| **Product FKs** | 2 (ManufacturerId, WholesalerId) | 1 (UserAccountId) |
| **Model Classes** | 3 separate | 1 unified |
| **Validation Logic** | Duplicated | Centralized |
| **Registration Steps** | 2-3 DB operations | 2-3 DB operations |
| **Data Queries** | Multiple includes | Single include |
| **Future Extensions** | Limited | Flexible |
| **Code Complexity** | Higher | Lower |
