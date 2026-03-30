using System.Data;
using Microsoft.Data.SqlClient;

namespace InsuranceSys.Infrastructure.Database.Interface
{
    /// <summary>
    /// Provides a single SqlConnection instance per HTTP request for the current tenant.
    /// Tenant resolution is automatic based on the authenticated user's claims.
    /// </summary>
    public interface ISqlConnectionProvider : IDisposable
    {
        /// <summary>
        /// Gets or creates the SqlConnection for the current tenant in this request scope
        /// Connection is opened automatically on first use
        /// </summary>
        Task<SqlConnection> GetConnectionAsync();
        
        /// <summary>
        /// Gets the connection string for the current tenant
        /// </summary>
        Task<string> GetConnectionStringAsync();
        
        /// <summary>
        /// Begins a new transaction on the shared connection
        /// </summary>
        Task<SqlTransaction> BeginTransactionAsync();

        /// <summary>
        /// Gets the current transaction if one is active
        /// </summary>
        SqlTransaction? CurrentTransaction { get; }
    }
}
