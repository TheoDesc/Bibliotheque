using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FilmLibrary.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IConfiguration _configuration;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Adresse e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirmation")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public string? AdminCode { get; set; }
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

        if (Input.Role != "User" && Input.Role != "Admin")
        {
            ModelState.AddModelError(
                "",
                "Le type de compte sélectionné est invalide.");

            return Page();
        }

        if (Input.Role == "Admin")
        {
            var expectedCode =
                _configuration["Registration:AdminCode"];

            if (string.IsNullOrWhiteSpace(expectedCode) ||
                Input.AdminCode != expectedCode)
            {
                ModelState.AddModelError(
                    nameof(Input.AdminCode),
                    "Code administrateur incorrect.");

                return Page();
            }
        }

        var user = new IdentityUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(
            user,
            Input.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description);
            }

            return Page();
        }

        if (!await _roleManager.RoleExistsAsync(Input.Role))
        {
            await _roleManager.CreateAsync(
                new IdentityRole(Input.Role));
        }

        await _userManager.AddToRoleAsync(
            user,
            Input.Role);

        await _signInManager.SignInAsync(
            user,
            isPersistent: false);

        return RedirectToAction(
            "Index",
            "Films");
    }
}