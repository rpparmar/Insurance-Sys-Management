using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSys.Domain.Entities
{
    public class InsuranceTypeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceTypeId { get; set; }
        public required string InsuranceType { get; set; }

        /// <summary>
        /// Form-template key (<c>MotorVehicle</c>, <c>Health</c>, <c>Life</c>,
        /// <c>PersonalAccident</c>, <c>Standard</c>). Not the primary key and must
        /// never be inferred from <see cref="InsuranceTypeId"/>.
        /// </summary>
        [MaxLength(64)]
        public string InsuranceTypeCode { get; set; } = string.Empty;

        /// <summary>CSS icon class (FontAwesome / Flaticon), e.g. <c>fas fa-car-side</c>.</summary>
        [MaxLength(128)]
        public string IconClass { get; set; } = string.Empty;

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        [NotMapped]
        public string? AssociationWithCompanyIDs { get; set; }
    }
}
