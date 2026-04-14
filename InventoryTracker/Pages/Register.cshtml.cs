using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using InventoryTracker.Data;
using InventoryTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryTracker.Pages;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _db;

    public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public string PasswordRequirements => "Password must be at least 6 characters and meet the configured Identity password rules.";

    public class InputModel
    {
        [Required]
        public string AccountType { get; set; } = "Manufacturer";

        [Required]
        [StringLength(75, MinimumLength = 8)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(65, MinimumLength = 8)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return Page();
        }

        if (Input.AccountType == "Manufacturer")
        {
            var m = new ManufacturerAccount
            {
                ManufacturerName = Input.CompanyName,
                ManufacturerEmail = Input.Email,
                AppUserId = user.Id
            };
            _db.ManufacturerAccounts.Add(m);
        }
        else
        {
            var w = new WholesalerAccount
            {
                WholesalerName = Input.CompanyName,
                WholesalerEmail = Input.Email,
                AppUserId = user.Id
            };
            _db.WholesalerAccounts.Add(w);
        }

        await _db.SaveChangesAsync();

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToPage("/Index");
    }
}
