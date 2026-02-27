using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database.Interface
{
    /// <summary>
    /// Provides a single DbContext instance per HTTP request for the current tenant
    /// This prevents creating multiple contexts per request and ensures proper disposal
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
        Task<string> GetConnectionStringAsync(bool masterDBConn = true);
    }
}
