namespace Insurancesys.web.Models
{
    /// <summary>Policies that use the Standard form template (common + payment partial).</summary>
    public class StandardPolicyViewModel : ICommonPolicyForm
    {
        public PolicyBasicDetailsViewModel BasicDetails { get; set; } = null!;
        public PolicyPaymentDetailsViewModel? PolicyPaymentDetails { get; set; }
    }
}
