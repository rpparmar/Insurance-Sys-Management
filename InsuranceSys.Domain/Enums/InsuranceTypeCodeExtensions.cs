namespace InsuranceSys.Domain.Enums
{
    public static class InsuranceTypeCodeExtensions
    {
        public static bool TryFromInsuranceTypeId(int id, out InsuranceTypeCode code)
        {
            if (Enum.IsDefined(typeof(InsuranceTypeCode), id))
            {
                code = (InsuranceTypeCode)id;
                return true;
            }

            code = default;
            return false;
        }

        /// <summary>Stable URL/DOM slug for customer policies grid (matches legacy literals for 1–4).</summary>
        public static string ToGridSlug(this InsuranceTypeCode code) => code switch
        {
            InsuranceTypeCode.MotorVehicle => "motor",
            InsuranceTypeCode.Health => "health",
            InsuranceTypeCode.Life => "life",
            InsuranceTypeCode.PersonalAccident => "personalaccident",
            InsuranceTypeCode.Travel => "travel",
            InsuranceTypeCode.HomeProperty => "home-property",
            InsuranceTypeCode.Fire => "fire",
            InsuranceTypeCode.Marine => "marine",
            InsuranceTypeCode.Commercial => "commercial",
            InsuranceTypeCode.Liability => "liability",
            InsuranceTypeCode.TermLife => "term-life",
            _ => code.ToString().ToLowerInvariant()
        };

        public static bool UsesStandardPoliciesList(this InsuranceTypeCode code) =>
            code != InsuranceTypeCode.MotorVehicle
            && code != InsuranceTypeCode.Health
            && code != InsuranceTypeCode.Life
            && code != InsuranceTypeCode.PersonalAccident;
    }
}
