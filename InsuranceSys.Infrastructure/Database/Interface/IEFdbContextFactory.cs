using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database.Interface
{
    public interface IEFdbContextFactory
    {
        EfdbContext CreateDbContext(string connectionString);        
    }
}
