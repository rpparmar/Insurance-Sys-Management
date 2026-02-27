using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database
{
    /// <summary>
    /// Base class for all tenant-aware repositories
    /// Provides access to the tenant's DbContext without creating multiple instances
    /// </summary>
    public abstract class SharedEFdbContextRepositoryBase
    {
        protected readonly IEFdbContextProvider _contextProvider;
        protected SharedEFdbContextRepositoryBase(
           IEFdbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;            
        }
        /// <summary>
        /// Executes a read operation (no tracking for better performance)
        /// </summary>
        protected async Task<TResult> ExecuteReadAsync<TResult>(
            Func<EfdbContext, Task<TResult>> operation)
        {
            // ✅ Gets shared context for this request (no new allocation if already created)
            var context = await _contextProvider.GetContextAsync();

            // Execute the operation
            return await operation(context);
        }

        /// <summary>
        /// Executes a write operation (with change tracking)
        /// </summary>
        protected async Task<TResult> ExecuteWriteAsync<TResult>(
            Func<EfdbContext, Task<TResult>> operation)
        {
            var context = await _contextProvider.GetContextAsync();

            var result = await operation(context);

            return result;
        }

        /// <summary>
        /// Executes an operation without returning a value
        /// </summary>
        protected async Task ExecuteAsync(Func<EfdbContext, Task> operation)
        {
            var context = await _contextProvider.GetContextAsync();
            await operation(context);
        }

        /// <summary>
        /// Direct access to context for complex scenarios
        /// </summary>
        protected async Task<EfdbContext> GetContextAsync()
        {
            return await _contextProvider.GetContextAsync();
        }
    }
}
