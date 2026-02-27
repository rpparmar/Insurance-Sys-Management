namespace Insurancesys.web.Models
{
    public class PolicyPaymentDetailsViewModel
    {
        // Payment Details
        public int PaymentId { get; set; }
        public string? PaymentMode { get; set; }
        public string? Transactionreferance { get; set; }
        public string? BankName { get; set; }
    }
}
