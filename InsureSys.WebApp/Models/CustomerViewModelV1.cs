using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class CustomerViewModelV1
    {
        public int CustomerID { get; set; }
        public int? LeadID { get; set; }
        [Required(ErrorMessage = "Enter first name")]
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AnniversaryDate { get; set; }
        public string? Gender { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "Please enter a valid 10-digit mobile number")]
        [Display(Name = "Mobile Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be exactly 10 digits")]
        public string Phone { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        #region Policy Details        
        [Required(ErrorMessage = "Enter policy number")]
        public string PolicyNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Select policy start date")]
        public DateTime? PolicyStartDate { get; set; }
        [Required(ErrorMessage = "Select policy due date")]
        public DateTime? PolicyDueDate { get; set; }
        public string PlanType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Select insurer")]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter grosss premium")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid amount entered")]
        [Range(0, 1000000.00)]
        public decimal? GrosssPremium { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid amount entered")]
        [Range(0, 1000000.00)]
        public decimal? NetPremium { get; set; }
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid amount entered")]
        [Range(0, 1000000.00)]
        public decimal? ODPremium { get; set; }
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid amount entered")]
        [Range(0, 1000000.00)]
        public decimal? NCB { get; set; }
        public string? Dealer { get; set; } = string.Empty;
        public string? SM { get; set; } = string.Empty;
        public IFormFile[]? DocumentFiles { get; set; }
        #endregion

        #region Vehicle details
        public string? Make { get; set; } = string.Empty;
        public string? VehicleModel { get; set; } = string.Empty;
        public string? Chassiseno { get; set; } = string.Empty;
        public string? Vehicleno { get; set; } = string.Empty;
        public string? Segment { get; set; } = string.Empty;
        public string? Fuel { get; set; } = string.Empty;
        public string? VehicleIDV { get; set; } = string.Empty;
        #endregion

        #region Payment details
        public string? PaymentMode { get; set; } = string.Empty;
        public string? Transactionreferance { get; set; } = string.Empty;
        public string? BankName { get; set; } = string.Empty;
        #endregion

        public string SubmitType { get; set; } = "Customer"; // "Customer" or "Full"
    }
}
