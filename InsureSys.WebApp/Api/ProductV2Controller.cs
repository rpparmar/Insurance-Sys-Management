using InsuranceSys.Application;
using InsuranceSys.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Insurancesys.web.Api.Models;

namespace Insurancesys.web.Api
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Use JWT authentication
    
    [ApiVersion("2.0")]
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/Product")]
    [ApiController]
    public class ProductV2Controller : ControllerBase
    {
        //private readonly IProductService _productService;
        //public ProductController(IProductService productService)
        //{
        //    _productService = productService;
        //}

        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new ProductV2Response
            {
                Message = "Products from V2",
                Features = new List<string> { "xml-support", "versioned-route" }
            });
        }
    }
}
