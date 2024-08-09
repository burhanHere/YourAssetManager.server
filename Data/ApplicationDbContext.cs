using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetAssignment> AssetAssignments { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<AssetSubCategory> AssetSubCategories { get; set; }
        public DbSet<AssetMaintenance> AssetMaintenances { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<LogAction> LogActions { get; set; }
        public DbSet<Vender> Venders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organization>()
                .HasOne(o => o.ApplicationUser)
                .WithMany(u => u.Organizations)
                .HasForeignKey(o => o.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Organization)
                .WithMany(o => o.AssetManagers)
                .HasForeignKey(a => a.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
            // Seed roles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "OrganizationOwner", ConcurrencyStamp = "1", NormalizedName = "ORGANIZATIONOWNER" },
                new IdentityRole { Id = "2", Name = "AssetManager", ConcurrencyStamp = "2", NormalizedName = "ASSETMANAGER" },
                new IdentityRole { Id = "3", Name = "Employee", ConcurrencyStamp = "3", NormalizedName = "EMPLOYEE" }
            );
        }
    }
}