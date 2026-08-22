using InsuranceSys.Application;
using InsuranceSys.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Insurancesys.web.Api.Models;
using Asp.Versioning;
namespace Insurancesys.web.Api
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Use JWT authentication
    
    [ApiVersion("1.0")]
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //private readonly IProductService _productService;
        //public ProductController(IProductService productService)
        //{
        //    _productService = productService;
        //}

        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new ProductV1Response
            {
                Message = "Products from V1"
            });
        }
    }
}
