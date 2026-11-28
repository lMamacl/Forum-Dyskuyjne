using Forum_Dyskusyjne.Models;
using Microsoft.AspNetCore.Identity;

namespace Forum_Dyskusyjne.Data;

public static class DbInitializer
{
    public static void Initialize(ForumDbContext context)
    {
        // EnsureCreated() automatycznie tworzy bazę i tabele, jeśli jeszcze nie istnieją
        context.Database.EnsureCreated();

        // Jeśli są już jakieś fora w bazie, nic nie robimy (dane już istnieją)
        if (context.Forums.Any())
        {
            return;
        }

        // 1. Dodajemy fora demonstracyjne
        var forumOgólne = new Forum
        {
            Name = "Ogólne Dyskusje",
            Description = "Miejsce na tematy codzienne i luźne rozmowy społeczności."
        };

        var forumProgramowanie = new Forum
        {
            Name = "Programowanie i .NET",
            Description = "Pytania, odpowiedzi i wymiana wiedzy o C#, ASP.NET oraz technologiach webowych."
        };

        context.Forums.AddRange(forumOgólne, forumProgramowanie);

        // 2. Dodajemy konta demonstracyjne (na razie prosty hash/hasło pod testy)
        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            Username = "Administrator",
            Email = "admin@forum.pl",
            Role = UserRole.Admin
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

        var user = new User
        {
            Username = "JanKowalski",
            Email = "user@forum.pl",
            Role = UserRole.User
        };
        user.PasswordHash = hasher.HashPassword(user, "User123!");

        context.Users.AddRange(admin, user);
        context.SaveChanges();

        // 3. Dodajemy startowy wątek z powitaniem
        var thread = new ForumThread
        {
            ForumId = forumOgólne.Id,
            Title = "Witajcie na nowym forum dyskusyjnym!",
            Content = "Oficjalny pierwszy wątek w naszym serwisie. Możesz dodawać własne odpowiedzi.",
            AuthorUsername = "Administrator"
        };

        context.Threads.Add(thread);
        context.SaveChanges();
    }
}