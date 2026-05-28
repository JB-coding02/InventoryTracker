using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace InventoryTracker.Authorization;

public sealed class UserRoleAuthorizationHandler(UserManager<ApplicationUser> userManager)
    : AuthorizationHandler<UserRoleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserRoleRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        ApplicationUser? currentUser = await userManager.GetUserAsync(context.User);
        if (currentUser != null && requirement.AllowedRoles.Contains(currentUser.UserRole))
        {
            context.Succeed(requirement);
        }
    }
}