using InsuranceSys.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.Interface
{
	public interface ILoginService
	{
		Task<UsersEntity?> GetUser(string username, string password);
	}
}
