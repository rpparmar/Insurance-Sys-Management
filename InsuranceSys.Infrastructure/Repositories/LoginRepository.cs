using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;


namespace InsuranceSys.Infrastructure.Repositories
{
    public class LoginRepository : EfRepositoryBase,ILoginService
    {
        private readonly IAppDBContext _dbcontext;
        public LoginRepository(IAppDBContext dbcontext
            , IEFdbContextFactory efdbContextFactory
            , IConnectionStringProvider connStringProvider
            ) : base(efdbContextFactory, connStringProvider)
        {
            _dbcontext = dbcontext;
        }

        public async Task<UsersEntity?> GetUser(string username, string password)
        {
            using var _efdbcontext = await CreateContextAsync();
            return await _efdbcontext.EFUsers
                            .FirstOrDefaultAsync(c => c.UserName == username && c.Password == password);
        }
    }
}
