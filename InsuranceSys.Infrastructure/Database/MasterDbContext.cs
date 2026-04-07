using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.Enums;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;

namespace InsuranceSys.Infrastructure.Database
{
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

        public DbSet<AgencyDetailsEntity> AgencyDetails { get; set; }
        public DbSet<AgencyUsersEntity> AgencyUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<AgencyDetailsEntity>(entity =>
            {
                entity.ToTable("AgencyDetails");
                entity.HasKey(e => e.AgencyId);

                entity.HasIndex(e => e.AgencyCode).IsUnique();
                entity.HasIndex(e => e.IsActive)
                      .HasFilter("[IsActive] = 1");

                entity.Property(e => e.AgencyCode).IsRequired(false);
                entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            });

            modelBuilder.Entity<AgencyUsersEntity>(entity =>
            {
                entity.ToTable("AgencyUsers");
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.AgencyId);

                entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasOne(e => e.AgencyDetails)
                      .WithMany()
                      .HasForeignKey(e => e.AgencyId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        /// <summary>
        /// Seeds the default SuperAdmin if not present. Called from Program.cs on startup.
        /// </summary>
        public async Task SeedSuperAdminAsync()
        {
            var existing = await AgencyUsers.FirstOrDefaultAsync(u => u.Username == "superadmin");

            if (existing == null)
            {
                var (hash, salt) = PasswordHasher.HashPassword("SuperAdmin@2026#");
                AgencyUsers.Add(new AgencyUsersEntity
                {
                    AgencyId = null,
                    Username = "superadmin",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Email = "admin@insuresys.local",
                    DisplayName = "System Administrator",
                    Role = (int)Roles.SuperAdmin,
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
