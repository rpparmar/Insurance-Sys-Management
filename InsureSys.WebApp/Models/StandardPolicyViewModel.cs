namespace Insurancesys.web.Models
{
    /// <summary>Non-motor policies that share the common + payment partial (types 5–11).</summary>
    public class StandardPolicyViewModel
    {
        public PolicyBasicDetailsViewModel BasicDetails { get; set; } = null!;
        public PolicyPaymentDetailsViewModel? PolicyPaymentDetails { get; set; }
    }
}
