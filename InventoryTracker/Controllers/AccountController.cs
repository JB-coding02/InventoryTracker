using InventoryTracker.Data;
using InventoryTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryTracker.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{

	// Dependencies are injected via constructor injection
	private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly ApplicationDbContext _db;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
		RoleManager<IdentityRole> roleManager,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
		_roleManager = roleManager;
        _db = db;
    }

    /// <summary>
    /// Displays the login form.
    /// </summary>
    /// <param name="returnUrl">Optional local URL to redirect to after successful authentication.</param>
    /// <returns>The login view.</returns>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    /// <summary>
    /// Processes a login request using the supplied credentials.
    /// </summary>
    /// <param name="model">Validated login form values.</param>
    /// <returns>A redirect on success; otherwise, the login view with validation messages.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Find user by email first
        ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        // Sign in using the user's actual username (not email)
        Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
            user.UserName ?? model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
            return View(model);
        }

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "Login is not allowed for this account.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    /// <summary>
    /// Displays the registration form.
    /// </summary>
    /// <returns>The registration view.</returns>
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    /// <summary>
    /// Creates a new application user and associated manufacturer or wholesaler profile.
    /// </summary>
    /// <param name="model">Validated registration form values.</param>
    /// <returns>A redirect on success; otherwise, the registration view with validation messages.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.AccountType == "Manufacturer")
        {
            if (await _db.UserAccounts.AnyAsync(u => u.AccountName == model.CompanyName && u.AccountRole == UserRole.Manufacturer))
            {
                ModelState.AddModelError(nameof(model.CompanyName), "A manufacturer with this name already exists.");
                return View(model);
            }

            if (await _db.UserAccounts.AnyAsync(u => u.AccountEmail == model.Email && u.AccountRole == UserRole.Manufacturer))
            {
                ModelState.AddModelError(nameof(model.Email), "A manufacturer with this email already exists.");
                return View(model);
            }
        }
        else if (model.AccountType == "Wholesaler")
        {
            if (await _db.UserAccounts.AnyAsync(u => u.AccountName == model.CompanyName && u.AccountRole == UserRole.Wholesaler))
            {
                ModelState.AddModelError(nameof(model.CompanyName), "A wholesaler with this name already exists.");
                return View(model);
            }

            if (await _db.UserAccounts.AnyAsync(u => u.AccountEmail == model.Email && u.AccountRole == UserRole.Wholesaler))
            {
                ModelState.AddModelError(nameof(model.Email), "A wholesaler with this email already exists.");
                return View(model);
            }
        }
        else
        {
            ModelState.AddModelError(nameof(model.AccountType), "Please select an account type.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (IdentityError err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(model);
        }

        // Assigns the user to the role, creating it first if it does not yet exist.
        // Handles the race condition where two concurrent registrations may both try to
        // create the same role simultaneously by treating DuplicateRoleName as non-fatal.
        async Task<IdentityResult> EnsureUserInRoleAsync(ApplicationUser applicationUser, string roleName)
        {
            var addToRoleResult = await _userManager.AddToRoleAsync(applicationUser, roleName);
            if (addToRoleResult.Succeeded)
            {
                return addToRoleResult;
            }

            IdentityResult createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!createRoleResult.Succeeded)
            {
                bool hasOnlyDuplicateRoleNameError = true;
                foreach (IdentityError error in createRoleResult.Errors)
                {
                    if (!string.Equals(error.Code, "DuplicateRoleName", StringComparison.OrdinalIgnoreCase))
                    {
                        hasOnlyDuplicateRoleNameError = false;
                        break;
                    }
                }

                if (!hasOnlyDuplicateRoleNameError)
                {
                    return createRoleResult;
                }
            }

            return await _userManager.AddToRoleAsync(applicationUser, roleName);
        }

        // Assign the user to the appropriate role based on their selected account type
        IdentityResult addUserToRoleResult = await EnsureUserInRoleAsync(user, model.AccountType);
        if (!addUserToRoleResult.Succeeded)
        {
            foreach (IdentityError err in addUserToRoleResult.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            await _userManager.DeleteAsync(user);
            return View(model);
        }

        try
        {
            UserRole userRole = model.AccountType == "Manufacturer" ? UserRole.Manufacturer : UserRole.Wholesaler;

            _db.UserAccounts.Add(new UserAccount
            {
                AccountName = model.CompanyName,
                AccountEmail = model.Email,
                AccountRole = userRole,
                AppUserId = user.Id
            });

            await _db.SaveChangesAsync();
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            ModelState.AddModelError(string.Empty, "Unable to complete registration. Please try again.");
            return View(model);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Logs out the currently signed-in user.
    /// </summary>
    /// <param name="returnUrl">Optional local URL to redirect to after logout.</param>
    /// <returns>A redirect to the home page or the specified return URL.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await _signInManager.SignOutAsync();

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
