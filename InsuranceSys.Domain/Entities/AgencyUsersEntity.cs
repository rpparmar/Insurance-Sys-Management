using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSys.Domain.Entities
{
    public class AgencyUsersEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public int? AgencyId { get; set; }

        [ForeignKey(nameof(AgencyId))]
        public AgencyDetailsEntity? AgencyDetails { get; set; }

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
        
        public int Role { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        
        public DateTime? LastLoginAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
