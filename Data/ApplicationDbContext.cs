using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace YourAssetManager.Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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