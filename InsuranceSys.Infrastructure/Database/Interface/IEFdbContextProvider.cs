namespace InsuranceSys.Infrastructure.Database.Interface
{
    /// <summary>
    /// Provides a single DbContext instance per HTTP request for the current tenant.
    /// Tenant resolution is automatic based on the authenticated user's claims.
    /// </summary>
    public interface IEFdbContextProvider : IDisposable
    {
        /// <summary>
        /// Gets or creates the DbContext for the current tenant in this request scope
        /// </summary>
        Task<EfdbContext> GetContextAsync();
        
        /// <summary>
        /// Gets the tenant's connection string
        /// </summary>
        Task<string> GetConnectionStringAsync();
    }
}
