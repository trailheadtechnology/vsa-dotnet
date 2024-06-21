using Microsoft.EntityFrameworkCore;
using Hotel.Api.Entities;

namespace Hotel.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    public DbSet<Reservation> Reservations { get; set; }
}
