using Insurancesys.web.Models;

namespace Insurancesys.web.PolicyForms
{
    /// <summary>Loaded policy row plus mapped payment, used by form handlers.</summary>
    public sealed class PolicyFormLoadContext
    {
        public required PolicyBasicDetailsViewModel BasicDetails { get; init; }
        public PolicyPaymentDetailsViewModel? Payment { get; init; }
        public int PolicyId { get; init; }
    }
}
