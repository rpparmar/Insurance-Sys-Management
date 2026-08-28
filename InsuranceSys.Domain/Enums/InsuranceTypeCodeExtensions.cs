using InsuranceSys.Domain.PolicyForms;

namespace InsuranceSys.Domain.Enums
{
    /// <summary>
    /// Compatibility helpers that delegate template metadata to <see cref="PolicyFormCatalog"/>.
    /// </summary>
    public static class InsuranceTypeCodeExtensions
    {
        /// <summary>Parses a stored key or alias into a form-template enum value.</summary>
        public static bool TryParseTemplate(string? key, out InsuranceTypeCode template)
        {
            if (PolicyFormCatalog.TryGet(key, out var definition)
                && Enum.TryParse(definition.Key, ignoreCase: true, out template))
            {
                return true;
            }

            template = InsuranceTypeCode.Standard;
            return false;
        }

        /// <summary>Resolves the form-template enum for a master row (legacy empty-key fallback included).</summary>
        public static InsuranceTypeCode ResolveTemplate(string? storedKey, int insuranceTypeId)
        {
            var definition = PolicyFormCatalog.ResolveDefinition(storedKey, insuranceTypeId);
            return Enum.TryParse(definition.Key, ignoreCase: true, out InsuranceTypeCode parsed)
                ? parsed
                : InsuranceTypeCode.Standard;
        }

        /// <summary>Maps alias enum values onto <see cref="InsuranceTypeCode.Standard"/>.</summary>
        public static InsuranceTypeCode NormalizeToFormTemplate(this InsuranceTypeCode code)
        {
            return PolicyFormCatalog.TryGet(code.ToString(), out var definition)
                && Enum.TryParse(definition.Key, ignoreCase: true, out InsuranceTypeCode parsed)
                ? parsed
                : InsuranceTypeCode.Standard;
        }

        /// <summary>Canonical key persisted on <c>InsuranceTypeMaster.InsuranceTypeCode</c>.</summary>
        public static string ToStoredKey(this InsuranceTypeCode code) =>
            NormalizeToFormTemplate(code).ToString();

        /// <summary>Default FontAwesome class when the master row has no icon.</summary>
        public static string GetDefaultIconClass(this InsuranceTypeCode code)
        {
            return PolicyFormCatalog.TryGet(code.ToString(), out var definition)
                ? definition.DefaultIconClass
                : PolicyFormCatalog.DefaultIconClass;
        }

        /// <summary>True when at most one active insurance type may use this template.</summary>
        public static bool UsesSpecializedForm(this InsuranceTypeCode code)
        {
            return PolicyFormCatalog.TryGet(code.ToString(), out var definition) && definition.IsSpecialized;
        }
    }
}
