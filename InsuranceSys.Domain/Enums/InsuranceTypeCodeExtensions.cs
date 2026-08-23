namespace InsuranceSys.Domain.Enums
{
    /// <summary>
    /// Form-template helpers for <see cref="InsuranceTypeCode"/>. Resolution is based on the
    /// stored template key, not on <c>InsuranceTypeId</c>.
    /// </summary>
    public static class InsuranceTypeCodeExtensions
    {
        /// <summary>
        /// Parses a stored master key into a form template. Travel / Home / Fire / Marine /
        /// Commercial / Liability / TermLife collapse to <see cref="InsuranceTypeCode.Standard"/>.
        /// </summary>
        /// <param name="key">Value from <c>InsuranceTypeMaster.InsuranceTypeCode</c>.</param>
        /// <param name="template">Resolved form template.</param>
        /// <returns><c>true</c> when <paramref name="key"/> is a known template or alias.</returns>
        public static bool TryParseTemplate(string? key, out InsuranceTypeCode template)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                template = InsuranceTypeCode.Standard;
                return false;
            }

            if (Enum.TryParse(key.Trim(), ignoreCase: true, out InsuranceTypeCode parsed))
            {
                template = NormalizeToFormTemplate(parsed);
                return true;
            }

            template = InsuranceTypeCode.Standard;
            return false;
        }

        /// <summary>
        /// Resolves the form template for a master row. Uses the stored key when present;
        /// empty keys fall back to the legacy seeded identities 1–4 only so existing
        /// tenant data keeps working before the SQL backfill runs.
        /// </summary>
        /// <param name="storedKey">Persisted <c>InsuranceTypeCode</c> column.</param>
        /// <param name="insuranceTypeId">Master primary key (legacy fallback only).</param>
        /// <returns>Normalized form template.</returns>
        public static InsuranceTypeCode ResolveTemplate(string? storedKey, int insuranceTypeId)
        {
            if (!string.IsNullOrWhiteSpace(storedKey))
            {
                return TryParseTemplate(storedKey, out var fromKey)
                    ? fromKey
                    : InsuranceTypeCode.Standard;
            }

            // Legacy seed rows created before the form-template column was populated.
            return insuranceTypeId switch
            {
                1 => InsuranceTypeCode.MotorVehicle,
                2 => InsuranceTypeCode.Health,
                3 => InsuranceTypeCode.Life,
                4 => InsuranceTypeCode.PersonalAccident,
                _ => InsuranceTypeCode.Standard
            };
        }

        /// <summary>
        /// Maps alias enum values (Travel, Fire, etc.) onto <see cref="InsuranceTypeCode.Standard"/>.
        /// </summary>
        public static InsuranceTypeCode NormalizeToFormTemplate(this InsuranceTypeCode code) =>
            code switch
            {
                InsuranceTypeCode.MotorVehicle => InsuranceTypeCode.MotorVehicle,
                InsuranceTypeCode.Health => InsuranceTypeCode.Health,
                InsuranceTypeCode.Life => InsuranceTypeCode.Life,
                InsuranceTypeCode.PersonalAccident => InsuranceTypeCode.PersonalAccident,
                _ => InsuranceTypeCode.Standard
            };

        /// <summary>
        /// Canonical key persisted on <c>InsuranceTypeMaster.InsuranceTypeCode</c>.
        /// </summary>
        public static string ToStoredKey(this InsuranceTypeCode code) =>
            NormalizeToFormTemplate(code).ToString();

        /// <summary>
        /// Unique DOM/JS slug. Specialized templates keep stable legacy slugs;
        /// Standard types include the master id so multiple Standard cards never collide.
        /// </summary>
        public static string ToUniqueSlug(this InsuranceTypeCode code, int insuranceTypeId)
        {
            var template = NormalizeToFormTemplate(code);
            return template switch
            {
                InsuranceTypeCode.MotorVehicle => "motor",
                InsuranceTypeCode.Health => "health",
                InsuranceTypeCode.Life => "life",
                InsuranceTypeCode.PersonalAccident => "personalaccident",
                _ => $"standard-{insuranceTypeId}"
            };
        }

        /// <summary>Stable URL/DOM slug for a template (Standard uses <c>standard</c>).</summary>
        public static string ToGridSlug(this InsuranceTypeCode code) =>
            NormalizeToFormTemplate(code) switch
            {
                InsuranceTypeCode.MotorVehicle => "motor",
                InsuranceTypeCode.Health => "health",
                InsuranceTypeCode.Life => "life",
                InsuranceTypeCode.PersonalAccident => "personalaccident",
                _ => "standard"
            };

        /// <summary>True when the template uses a dedicated partial (not Standard).</summary>
        public static bool UsesSpecializedForm(this InsuranceTypeCode code) =>
            NormalizeToFormTemplate(code) is InsuranceTypeCode.MotorVehicle
                or InsuranceTypeCode.Health
                or InsuranceTypeCode.Life
                or InsuranceTypeCode.PersonalAccident;

        /// <summary>True when policies bind to <c>StandardPolicies</c>.</summary>
        public static bool UsesStandardPoliciesList(this InsuranceTypeCode code) =>
            !code.UsesSpecializedForm();

        /// <summary>ASP.NET model-binding collection name.</summary>
        public static string GetFormCollectionPrefix(this InsuranceTypeCode code) =>
            NormalizeToFormTemplate(code) switch
            {
                InsuranceTypeCode.MotorVehicle => "MotorPolicies",
                InsuranceTypeCode.Health => "HealthPolicies",
                InsuranceTypeCode.Life => "LifePolicies",
                InsuranceTypeCode.PersonalAccident => "PersonalAccidentPolicies",
                _ => "StandardPolicies"
            };

        /// <summary>
        /// Value assigned to <c>ViewBag.PolicyType</c> so shared partials emit
        /// <c>{PolicyType}Policies[{index}].…</c> field names.
        /// </summary>
        public static string GetBindingPolicyType(this InsuranceTypeCode code) =>
            NormalizeToFormTemplate(code) switch
            {
                InsuranceTypeCode.MotorVehicle => "Motor",
                InsuranceTypeCode.Health => "Health",
                InsuranceTypeCode.Life => "Life",
                InsuranceTypeCode.PersonalAccident => "PersonalAccident",
                _ => "Standard"
            };

        /// <summary>Razor partial that renders this template.</summary>
        public static string GetPartialViewName(this InsuranceTypeCode code) =>
            NormalizeToFormTemplate(code) switch
            {
                InsuranceTypeCode.MotorVehicle => "_MotorPolicyPartial",
                InsuranceTypeCode.Health => "_HealthPolicyPartial",
                InsuranceTypeCode.Life => "_LifePolicyPartial",
                InsuranceTypeCode.PersonalAccident => "_PersonalAccidentPolicyPartial",
                _ => "_StandardPolicyPartial"
            };

        /// <summary>Default FontAwesome class when the master row has no icon.</summary>
        public static string GetDefaultIconClass(this InsuranceTypeCode code) =>
            NormalizeToFormTemplate(code) switch
            {
                InsuranceTypeCode.MotorVehicle => "fas fa-car-side",
                InsuranceTypeCode.Health => "fas fa-heartbeat",
                InsuranceTypeCode.Life => "fas fa-shield-alt",
                InsuranceTypeCode.PersonalAccident => "fas fa-ambulance",
                _ => "fas fa-file-alt"
            };
    }
}
