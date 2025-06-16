using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database.Interface
{
    public interface IConnectionStringProvider
    {
        Task<string> GetConnectionStringAsync(bool masterDBConn=true);
    }
}
