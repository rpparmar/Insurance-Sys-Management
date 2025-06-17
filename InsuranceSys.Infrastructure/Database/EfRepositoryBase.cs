using InsuranceSys.Infrastructure.Database.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database
{
    public abstract class EfRepositoryBase
    {
        private readonly IEFdbContextFactory _dbContextFactory;
        private readonly IConnectionStringProvider _connStringProvider;

        protected EfRepositoryBase(
            IEFdbContextFactory dbContextFactory,
            IConnectionStringProvider connStringProvider)
        {
            _dbContextFactory = dbContextFactory;
            _connStringProvider = connStringProvider;
        }

        protected async Task<EfdbContext> CreateContextAsync()
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            return _dbContextFactory.CreateDbContext(connStr);
        }
    }
}
