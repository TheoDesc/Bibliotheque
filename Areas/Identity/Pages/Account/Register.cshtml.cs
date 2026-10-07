using System.ComponentModel.DataAnnotations;
using FilmLibrary.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FilmLibrary.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "L'adresse e-mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        [Display(Name = "Adresse e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [StringLength(
            100,
            ErrorMessage = "Le mot de passe doit contenir au moins {2} caractères.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez choisir un type de compte.")]
        [Display(Name = "Type de compte")]
        public string Role { get; set; } = DbInitializer.UserRole;

        [Display(Name = "Code administrateur")]
        public string? AdminCode { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input.Role != DbInitializer.UserRole &&
            Input.Role != DbInitializer.AdminRole)
        {
            ModelState.AddModelError(
                "Input.Role",
                "Le type de compte sélectionné est invalide.");
        }

        if (Input.Role == DbInitializer.AdminRole)
        {
            var expectedCode = _configuration["Registration:AdminCode"];

            if (string.IsNullOrWhiteSpace(expectedCode) ||
                Input.AdminCode != expectedCode)
            {
                ModelState.AddModelError(
                    "Input.AdminCode",
                    "Le code administrateur est incorrect.");
            }
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new IdentityUser
        {
            UserName = Input.Email,
            Email = Input.Email
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        var role = Input.Role == DbInitializer.AdminRole
            ? DbInitializer.AdminRole
            : DbInitializer.UserRole;

        var roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        // Connexion automatique après inscription
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToPage("/Index");
    }
}