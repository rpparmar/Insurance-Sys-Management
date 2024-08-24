using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application
{
    public interface IProductService
    {
        List<Domain.Product> GetAllProducts();
    }
}
