using donate.Models;
using Microsoft.EntityFrameworkCore;
namespace donate.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) :base(opt)
        {            
        }
        public DbSet<Donation> Donations { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Request> Requests { get; set; } = null!;
        public DbSet<Category> Categorys { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Donation -> User (One-to-Many)
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany(u => u.Donations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes here

            // Configure Request -> User (One-to-Many)
            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany(u => u.Requests)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes here

            // Configure Request -> Donation (One-to-Many)
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Donation)
                .WithMany(d => d.Requests)
                .HasForeignKey(r => r.DonationId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes here

            // Optional: Add unique constraints, indexes, etc.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Example: Ensuring Email is unique
        }
    }
}
