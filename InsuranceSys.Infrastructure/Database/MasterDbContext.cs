using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;

namespace InsuranceSys.Infrastructure.Database
{
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

        public DbSet<TenantEntity> Tenants { get; set; }
        public DbSet<TenantUserEntity> TenantUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TenantEntity>(entity =>
            {
                entity.ToTable("Tenants");
                entity.HasKey(e => e.TenantId);

                entity.HasIndex(e => e.TenantCode).IsUnique();
                entity.HasIndex(e => e.IsActive)
                      .HasFilter("[IsActive] = 1");

                entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            });

            modelBuilder.Entity<TenantUserEntity>(entity =>
            {
                entity.ToTable("TenantUsers");
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.TenantId);

                entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasOne(e => e.Tenant)
                      .WithMany()
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        /// <summary>
        /// Seeds the default SuperAdmin if not present. Called from Program.cs on startup.
        /// </summary>
        public async Task SeedSuperAdminAsync()
        {
            var existing = await TenantUsers.FirstOrDefaultAsync(u => u.Username == "superadmin");

            if (existing == null)
            {
                var (hash, salt) = PasswordHasher.HashPassword("SuperAdmin@2026#");
                TenantUsers.Add(new TenantUserEntity
                {
                    TenantId = null,
                    Username = "superadmin",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Email = "admin@insuresys.local",
                    DisplayName = "System Administrator",
                    Role = "SuperAdmin",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
                await SaveChangesAsync();
            }
            else if (existing.PasswordHash == "PENDING_APP_SEED")
            {
                var (hash, salt) = PasswordHasher.HashPassword("SuperAdmin@2026#");
                existing.PasswordHash = hash;
                existing.PasswordSalt = salt;
                existing.UpdatedAtUtc = DateTime.UtcNow;
                await SaveChangesAsync();
            }
        }
    }
}
