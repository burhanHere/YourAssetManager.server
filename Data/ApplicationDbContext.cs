using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // Define your DbSet properties
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationDomains> OrganizationDomains { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<AssetStatus> AssetStatuses { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetReturn> AssetReturns { get; set; }
        public DbSet<AssetRetire> AssetRetires { get; set; }
        public DbSet<AssetMaintenance> AssetMaintenances { get; set; }
        public DbSet<AssetAssignment> AssetAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.AssetIdentificationNumber)
                .IsUnique();

            // Seed roles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "OrganizationOwner", ConcurrencyStamp = "1", NormalizedName = "ORGANIZATIONOWNER" },
                new IdentityRole { Id = "2", Name = "AssetManager", ConcurrencyStamp = "2", NormalizedName = "ASSETMANAGER" },
                new IdentityRole { Id = "3", Name = "Employee", ConcurrencyStamp = "3", NormalizedName = "EMPLOYEE" }
            );

            // Seed Status
            modelBuilder.Entity<AssetStatus>().HasData(
                new AssetStatus { Id = 1, StatusName = "Assigned" },
                new AssetStatus { Id = 2, StatusName = "Retired" },
                new AssetStatus { Id = 3, StatusName = "UnderMaintenance" },
                new AssetStatus { Id = 4, StatusName = "Available" }
            );
        }
    }
}
