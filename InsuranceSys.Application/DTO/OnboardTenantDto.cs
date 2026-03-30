namespace InsuranceSys.Application.DTO
{
    public class OnboardTenantDto
    {
        public string TenantCode { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string DesiredDatabaseName { get; set; } = string.Empty;
        public string AdminUsername { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
