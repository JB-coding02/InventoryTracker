using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryTracker.Controllers;

/// <summary>
/// Manages user profile operations including viewing, editing, and deleting user accounts.
/// Requires authentication. Provides functionality for users to manage their profile information,
/// change roles (with restrictions for non-admin users), and delete their accounts.
/// </summary>
[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Displays the current user's profile information.
    /// </summary>
    /// <returns>The profile view with the authenticated user's details, or redirects to login if user is not found.</returns>
    public async Task<IActionResult> Index()
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        return View(user);
    }

    /// <summary>
    /// Displays the edit profile form pre-populated with the current user's information.
    /// Only admin users can modify their own role; non-admin users cannot assign themselves the Admin role.
    /// </summary>
    /// <returns>The edit profile view with the user's current information, or redirects to login if user is not found.</returns>
    public async Task<IActionResult> EditProfile()
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        EditProfileViewModel model = new EditProfileViewModel
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            CompanyName = user.CompanyName,
            UserRole = user.UserRole
        };

        ViewBag.IsAdmin = user.UserRole == UserRole.Admin;

        return View(model);
    }

    /// <summary>
    /// Saves profile changes for the authenticated user including username, email, phone number, company name, and user role.
    /// Validates that usernames and emails are unique (excluding the current user) and prevents non-admin users from
    /// assigning themselves an Admin role.
    /// </summary>
    /// <param name="model">The updated profile information.</param>
    /// <returns>A redirect to the identity management page on success; otherwise returns the edit profile view with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        // Prevent non-admin users from changing their role to Admin
        if (user.UserRole != UserRole.Admin && model.UserRole == UserRole.Admin)
        {
            ModelState.AddModelError("UserRole", "You cannot assign yourself an Admin role.");
            return View(model);
        }

        // Update username if it has changed
        if (user.UserName != model.UserName)
        {
            ApplicationUser? existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                ModelState.AddModelError("UserName", "This username is already taken.");
                return View(model);
            }

            user.UserName = model.UserName;
        }

        // Update email if it has changed
        if (user.Email != model.Email)
        {
            ApplicationUser? existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                return View(model);
            }

            user.Email = model.Email;
        }

        // Update phone number, company name, and user role
        user.PhoneNumber = model.PhoneNumber;
        user.CompanyName = model.CompanyName;
        user.UserRole = model.UserRole;

        IdentityResult result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Your profile has been updated successfully.";
            return Redirect("/Identity/Account/Manage");
        }

        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    /// <summary>
    /// Permanently deletes the authenticated user's account from the system.
    /// </summary>
    /// <returns>A redirect to the login page on success; otherwise returns to the edit profile page with error messages.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        IdentityResult result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Redirect("/Identity/Account/Login");
        }

        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return RedirectToAction("EditProfile");
    }
}

