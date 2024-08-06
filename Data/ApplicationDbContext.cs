using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<Organization> Organizations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organization>()
             .HasOne(o => o.OrganizationOwner)
             .WithMany(u => u.Organizations)
             .HasForeignKey(o => o.OrganizationOwnerId) // Corrected property name
             .OnDelete(DeleteBehavior.Cascade);

            SeedRoles(modelBuilder);
        }
        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                    new() { Name = "OrganizationOwner", ConcurrencyStamp = "1", NormalizedName = "ORGANIZATIONOWNER" },
                    new() { Name = "AssetManager", ConcurrencyStamp = "2", NormalizedName = "ASSETMANAGER" },
                    new() { Name = "Employee", ConcurrencyStamp = "3", NormalizedName = "EMPLOYEE" }
                    );
        }
    }
}