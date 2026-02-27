using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;


namespace InsuranceSys.Infrastructure.Repositories
{
    public class LoginRepository : SharedEFdbContextRepositoryBase, ILoginService
    {
        private readonly IAdoNetDBContext _dbcontext;
        public LoginRepository(IEFdbContextProvider contextProvider, IAdoNetDBContext dbcontext) : base(contextProvider)
        {
            _dbcontext = dbcontext;
        }

        public async Task<UsersEntity?> GetUser(string username, string password)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //return await _efdbcontext.EFUsers
            //                .FirstOrDefaultAsync(c => c.UserName == username && c.Password == password); 
            #endregion

            return await ExecuteReadAsync(async context =>
            {
                return await context.EFUsers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.UserName == username && c.Password == password);
            });
        }
    }
}
