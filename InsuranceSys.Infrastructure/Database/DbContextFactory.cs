using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database
{
    public class DbContextFactory
    {
        public EfdbContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EfdbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new EfdbContext(optionsBuilder.Options);
        }
    }
}
