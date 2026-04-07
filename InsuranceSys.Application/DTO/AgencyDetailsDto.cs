namespace InsuranceSys.Application.DTO
{
    public class AgencyDetailsDto
    {
        public int AgencyId { get; set; }
        public string? AgencyCode { get; set; }
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
