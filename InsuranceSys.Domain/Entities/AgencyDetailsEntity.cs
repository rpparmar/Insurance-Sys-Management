using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSys.Domain.Entities
{
    public class AgencyDetailsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AgencyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AgencyCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string AgencyName { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? ContactEmail { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [Required]
        [MaxLength(128)]
        public string DatabaseName { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string DatabaseServer { get; set; } = string.Empty;

        [MaxLength(128)]
        public string? DatabaseUser { get; set; }

        [MaxLength(512)]
        public string? EncryptedDatabasePassword { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }

        public int? CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public AgencyUsersEntity? CreatedByUser { get; set; }
    }
}
