using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserConnection> UserConnections { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
}
