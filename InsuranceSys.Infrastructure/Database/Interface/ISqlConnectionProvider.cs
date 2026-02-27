using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database.Interface
{
    /// <summary>
    /// Provides a single SqlConnection instance per HTTP request for the current tenant
    /// This prevents creating multiple connections per request and ensures proper disposal
    /// </summary>
    public interface ISqlConnectionProvider: IDisposable
    {
        /// <summary>
        /// Gets or creates the SqlConnection for the current tenant in this request scope
        /// Connection is opened automatically on first use
        /// </summary>
        Task<SqlConnection> GetConnectionAsync();

        /// <summary>
        /// Gets the connection string for the current tenant
        /// </summary>
        Task<string> GetConnectionStringAsync(bool masterDBConn = true);

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
