using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagement.Models;
using UserManagement.ViewModels;

namespace UserManagement.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<UserDetails> _userManager;
    private readonly SignInManager<UserDetails> _signInManager;
    private readonly DBContext _db;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<UserDetails> userManager,
        SignInManager<UserDetails> signInManager,
        DBContext db,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("Name,Email,Password,ConfirmPassword")] RegisterViewModel model)
    {
        _logger.LogInformation("Registering user with email: {Email}", model?.Email);
        if (model == null)
        {
            _logger.LogError("Register model is null");
            return BadRequest("Invalid registration data.");
        }
        
        if (!ModelState.IsValid) return View(model);

        var user = new UserDetails
        {
            Name = model.Name,
            Email = model.Email,
            UserName = model.Email,
            IsBlocked = false,
            LastLoginDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User registered successfully: {Email}", model.Email);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
            _logger.LogWarning("Identity error: {Code} - {Description}", error.Code, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in: {Email}", model.Email);
            return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
        }

        _logger.LogWarning("Failed login attempt for user: {Email}", model.Email);
        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }
}
