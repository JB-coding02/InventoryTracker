using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;

namespace InventoryTracker.Authorization;

public sealed class UserRoleRequirement : IAuthorizationRequirement
{
    public UserRoleRequirement(params UserRole[] allowedRoles)
    {
        AllowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
    }

    public IReadOnlyCollection<UserRole> AllowedRoles { get; }
}