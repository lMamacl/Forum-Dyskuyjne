using System.Security.Claims;
using Forum_Dyskusyjne.Data;
using Forum_Dyskusyjne.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Forum_Dyskusyjne.Controllers;

public class AccountController : Controller
{
    private readonly ForumDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public AccountController(ForumDbContext context, IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    // ================================================================
    // AKCJA 1: Register()
    // URL:     GET /Account/Register  lub  POST /Account/Register
    // WIDOK:   Views/Account/Register.cshtml
    // ZADANIE: Rejestracja nowego użytkownika, haszowanie hasła i obsługa błędów.
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Sprawdzenie czy użytkownik już istnieje
        if (await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
        {
            ModelState.AddModelError("", "Użytkownik o podanej nazwie lub adresie email już istnieje.");
            return View(model);
        }

        // Utworzenie obiektu użytkownika i domyślne przypisanie roli User
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };
        
        // Bezpieczne haszowanie hasła algorytmem PBKDF2 przed zapisem do bazy
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    // ================================================================
    // AKCJA 2: Login()
    // URL:     GET /Account/Login  lub  POST /Account/Login
    // WIDOK:   Views/Account/Login.cshtml
    // ZADANIE: Logowanie użytkownika do sesji przeglądarki (Cookies) i tworzenie Claimów.
    [HttpGet]
    public IActionResult Login(string? returnUrl = null, string? reason = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["Reason"] = reason;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
         ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
        if (user == null)
        {
            // BEZPIECZEŃSTWO: Generyczny komunikat zapobiega sprawdzaniu czy dany login istnieje w bazie
            ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika lub hasło.");
            return View(model);
        }

        // Weryfikacja zgodności wprowadzonego hasła z haszem w bazie danych
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika lub hasło.");
            return View(model);
        }

        // Tworzenie claims (tożsamości) użytkownika dla ciasteczka
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()) // Wymagane do działania [Authorize(Roles = "Admin")]
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        // Zapisanie zaszyfrowanego ciasteczka uwierzytelniającego w przeglądarce klienta
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    // ================================================================
    // AKCJA 3: Logout()
    // URL:     POST /Account/Logout
    // WIDOK:   Brak (Przekierowanie)
    // ZADANIE: Czyszczenie sesji i usunięcie ciasteczka uwierzytelniającego.
    [HttpPost]
    [ValidateAntiForgeryToken] // Zabezpieczenie przed niechcianym wylogowaniem przez ataki CSRF
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
