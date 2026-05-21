using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryTracker.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class IndexModel(UserManager<ApplicationUser> userManager) : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;

	public ApplicationUser? CurrentUser { get; private set; }

	[BindProperty]
	public string? NewUsername { get; set; }

	[BindProperty]
	public UserRole? NewUserRole { get; set; }

	public string? StatusMessage { get; set; }
	public string? ErrorMessage { get; set; }

	public async Task OnGetAsync()
	{
		CurrentUser = await _userManager.GetUserAsync(User);
		if (CurrentUser != null)
		{
			NewUsername = CurrentUser.UserName;
			NewUserRole = CurrentUser.UserRole;
		}
	}

	public async Task<IActionResult> OnPostUpdateUsernameAsync()
	{
		CurrentUser = await _userManager.GetUserAsync(User);
		if (CurrentUser == null)
		{
			return NotFound();
		}

		if (string.IsNullOrWhiteSpace(NewUsername))
		{
			ErrorMessage = "Username cannot be empty.";
			NewUserRole = CurrentUser.UserRole;
			return Page();
		}

		if (NewUsername != CurrentUser.UserName)
		{
			var existingUser = await _userManager.FindByNameAsync(NewUsername);
			if (existingUser != null && existingUser.Id != CurrentUser.Id)
			{
				ErrorMessage = "This username is already taken.";
				NewUserRole = CurrentUser.UserRole;
				return Page();
			}

			CurrentUser.UserName = NewUsername;
			var result = await _userManager.UpdateAsync(CurrentUser);
			if (!result.Succeeded)
			{
				ErrorMessage = "Failed to update username.";
				NewUserRole = CurrentUser.UserRole;
				return Page();
			}

			StatusMessage = "Username updated successfully.";
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostUpdateUserRoleAsync()
	{
		CurrentUser = await _userManager.GetUserAsync(User);
		if (CurrentUser == null)
		{
			return NotFound();
		}

		if (NewUserRole == null)
		{
			ErrorMessage = "Invalid role selection.";
			NewUsername = CurrentUser.UserName;
			return Page();
		}

		// Non-Admin accounts can only switch between Manufacturer and Wholesaler
		if (CurrentUser.UserRole != UserRole.Admin && NewUserRole == UserRole.Admin)
		{
			ErrorMessage = "You cannot assign yourself an Admin role.";
			NewUsername = CurrentUser.UserName;
			return Page();
		}

		if (CurrentUser.UserRole != NewUserRole)
		{
			CurrentUser.UserRole = NewUserRole.Value;
			var result = await _userManager.UpdateAsync(CurrentUser);
			if (!result.Succeeded)
			{
				ErrorMessage = "Failed to update user role.";
				NewUsername = CurrentUser.UserName;
				return Page();
			}

			StatusMessage = "User role updated successfully.";
		}

		return RedirectToPage();
	}
}
