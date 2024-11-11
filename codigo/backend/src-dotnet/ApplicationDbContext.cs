using Microsoft.EntityFrameworkCore;

namespace Backend.Data;
using Backend.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Stores> Store { get; set; }
    public DbSet<Inventory> Inventory { get; set; }
    public DbSet<Products> Product { get; set; }
    public DbSet<AvailableCeps> AvailableCeps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AvailableCeps>()
            .Property(a => a.Cep)
            .HasMaxLength(9);

        modelBuilder.Entity<Stores>()
            .Property(s => s.Cep)
            .HasMaxLength(9);
    }
}
