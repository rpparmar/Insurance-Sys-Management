namespace Insurancesys.web.PolicyForms
{
    /// <summary>Looks up <see cref="IPolicyFormHandler"/> instances by template key.</summary>
    public interface IPolicyFormRegistry
    {
        /// <summary>All registered handlers.</summary>
        IReadOnlyCollection<IPolicyFormHandler> All { get; }

        /// <summary>Returns the handler for <paramref name="templateKey"/>, or Standard when unknown.</summary>
        IPolicyFormHandler Get(string? templateKey);

        /// <summary>Tries to resolve a handler without falling back to Standard.</summary>
        bool TryGet(string? templateKey, out IPolicyFormHandler handler);
    }
}
