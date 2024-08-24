using InsuranceSys.Application;
using InsuranceSys.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Insurancesys.web.Api
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Use JWT authentication
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<IList<Product>> Get()
        {
            return Ok(this._productService.GetAllProducts());
        }
    }
}
