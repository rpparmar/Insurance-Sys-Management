using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;


namespace InsuranceSys.Infrastructure
{
    public class LoginRepository:ILoginService
    {
		private readonly IAppDBContext _dbcontext;		
        private readonly IEFdbContextFactory _efdbContextFactory;
        private readonly IConnectionStringProvider _connStringProvider;
        public LoginRepository(IAppDBContext dbcontext            
            , IEFdbContextFactory efdbContextFactory
            , IConnectionStringProvider connStringProvider
            ) 
        {			
			_dbcontext = dbcontext;
            _efdbContextFactory = efdbContextFactory;
            _connStringProvider = connStringProvider;
        }

        public async Task<UsersEntity?> GetUser(string username, string password)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            return await _efdbcontext.EFUsers
                            .FirstOrDefaultAsync(c => c.UserName == username && c.Password==password);			

		}

    }
}
