using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Forum_Dyskusyjne.Data;
using Forum_Dyskusyjne.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Forum_Dyskusyjne.Controllers;

public class AccountController : Controller
{
    private readonly ForumDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;

    public AccountController(ForumDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
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
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

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

    // ================================================================
    // AKCJA 4: GenerateToken()
    // URL:     POST /Account/Token
    // WIDOK:   Brak (Zwraca obiekt JSON dla klientów API / Postmana)
    // ZADANIE: Weryfikacja danych i wygenerowanie zabezpieczonego tokenu JWT dla zewnętrznych żądań.
    [HttpPost]
    [Route("Account/Token")]
    public async Task<IActionResult> GenerateToken([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
        if (user == null) return Unauthorized("Nieprawidłowa nazwa użytkownika lub hasło.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized("Nieprawidłowa nazwa użytkownika lub hasło.");

        // Pobranie tajnej konfiguracji klucza i parametrów z appsettings.json / secrets.json
        var secretKey = _configuration["JwtSettings:SecretKey"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expiryInMinutes = Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"] ?? "60");

        if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
        {
            return StatusCode(500, "Błąd konfiguracji serwera: Klucz JWT jest za krótki lub nie istnieje.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Tworzenie claims dla tokenu JWT - użycie ClaimTypes.Role zapewnia pełną spójność z systemem Cookie
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()) 
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: creds
        );

        // Zwrócenie wygenerowanego tokenu oraz czasu jego wygaśnięcia w formacie JSON
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }
}
