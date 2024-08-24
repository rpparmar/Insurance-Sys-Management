using InsuranceSys.Application;
using InsuranceSys.Infrastructure;

namespace InsuranceSys.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly IAppDBContext _dbcontext;
        public ProductRepository(IAppDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public static List<Domain.Product> lstProducts = new List<Domain.Product>()
        {
           new Domain.Product{  Id =1 ,Name= "Shampoo", Type ="G" },
           new Domain.Product{  Id =2 ,Name= "Bathing Soap", Type ="S"},
           new Domain.Product{  Id =3 ,Name= "Face Wash", Type ="G"},
           new Domain.Product{  Id =4 ,Name= "Detergent", Type ="S"},

        };

        public List<Domain.Product> GetAllProducts()
        {
            return lstProducts;
        }
    }
}