namespace InsuranceSys.Application.DTO
{
    public class TenantConnectionInfo
    {
        public int TenantId { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public string DatabaseServer { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }
}
