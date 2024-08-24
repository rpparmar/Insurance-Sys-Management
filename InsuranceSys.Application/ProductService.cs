using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InsuranceSys.Application
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }
        List<Domain.Product> IProductService.GetAllProducts()
        {
            return this._productRepository.GetAllProducts();
        }
    }
}
