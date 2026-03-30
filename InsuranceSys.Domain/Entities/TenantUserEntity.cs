using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSys.Domain.Entities
{
    public class TenantUserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public int? TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public TenantEntity? Tenant { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public string PasswordSalt { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? DisplayName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
