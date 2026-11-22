using Forum_Dyskusyjne.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum_Dyskusyjne.Data;

public class ForumDbContext : DbContext
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
    {
    }

    // Te 4 właściwości odpowiadają za tabele w bazie danych:
    public DbSet<User> Users => Set<User>();
    public DbSet<Forum> Forums => Set<Forum>();
    public DbSet<ForumThread> Threads => Set<ForumThread>();
    public DbSet<ForumPost> Posts => Set<ForumPost>();
}