using InsuranceSys.Domain.PolicyForms;

namespace Insurancesys.web.PolicyForms
{
    /// <summary>DI-backed dictionary of <see cref="IPolicyFormHandler"/> keyed by template.</summary>
    public sealed class PolicyFormRegistry : IPolicyFormRegistry
    {
        private readonly Dictionary<string, IPolicyFormHandler> _handlers;

        public PolicyFormRegistry(IEnumerable<IPolicyFormHandler> handlers)
        {
            _handlers = handlers.ToDictionary(
                handler => handler.TemplateKey,
                StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IPolicyFormHandler> All => _handlers.Values;

        /// <inheritdoc />
        public IPolicyFormHandler Get(string? templateKey)
        {
            if (TryGet(templateKey, out var handler))
                return handler;

            return _handlers[PolicyFormCatalog.StandardKey];
        }

        /// <inheritdoc />
        public bool TryGet(string? templateKey, out IPolicyFormHandler handler)
        {
            if (PolicyFormCatalog.TryGet(templateKey, out var definition)
                && _handlers.TryGetValue(definition.Key, out handler!))
            {
                return true;
            }

            handler = null!;
            return false;
        }
    }
}
