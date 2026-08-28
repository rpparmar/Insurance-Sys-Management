namespace Insurancesys.web.Models
{
    /// <summary>Shared shape for policy form view models (basic + payment).</summary>
    public interface ICommonPolicyForm
    {
        PolicyBasicDetailsViewModel BasicDetails { get; set; }
        PolicyPaymentDetailsViewModel? PolicyPaymentDetails { get; set; }
    }
}
