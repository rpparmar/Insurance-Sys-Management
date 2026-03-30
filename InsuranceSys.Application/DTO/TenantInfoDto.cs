namespace InsuranceSys.Application.DTO
{
    public class TenantInfoDto
    {
        public int TenantId { get; set; }
        public string TenantCode { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public string DatabaseServer { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedByUsername { get; set; }
    }
}
