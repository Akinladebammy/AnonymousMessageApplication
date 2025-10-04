using AnonymousMessageApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace AnonymousMessageApplication.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed SuperAdmin
            var superAdmin = new Admin
            {
                Id = Guid.NewGuid(),
                Username = "superadmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Superadmin123"),
                Role = "SuperAdmin"
            };

            modelBuilder.Entity<Admin>().HasData(superAdmin);
        }
    }
}
