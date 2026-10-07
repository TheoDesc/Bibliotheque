using FilmLibrary.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilmLibrary.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Film> Films { get; set; } = null!;

    public DbSet<Genre> Genres { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;

    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Film>()
            .HasOne(f => f.Genre)
            .WithMany(g => g.Films)
            .HasForeignKey(f => f.GenreId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderItem>()
            .HasOne(i => i.Film)
            .WithMany()
            .HasForeignKey(i => i.FilmId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}