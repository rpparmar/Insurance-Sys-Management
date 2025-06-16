using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database
{
    public class ConnectionStringProvider: IConnectionStringProvider
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<string> GetConnectionStringAsync(bool masterDBConn = true)
        {
            // Option 1: From config file
            var connStr = _configuration.GetConnectionString("MasterConnection");
            if (!masterDBConn)
                connStr = _configuration.GetConnectionString("TestConnection");

            // Option 2: Lookup from central config DB or cache
            if (string.IsNullOrEmpty(connStr))
                throw new Exception("Invalid client");

            return Task.FromResult(connStr);
        }
    }
}
