using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.Entities
{
    public class PolicyPaymentDetailsEntity
    {
        // Payment Details
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int PolicyId { get; set; }
        public string? PaymentMode { get; set; }
        public string? Transactionreferance { get; set; }
        public string? BankName { get; set; }
    }
}
