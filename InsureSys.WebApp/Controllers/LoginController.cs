using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Insurancesys.web.Controllers
{
	public class LoginController : Controller
	{
		private readonly IMapper _mapper;
		private readonly ILoginService _loginService;
		public LoginController(IMapper mapper, ILoginService loginService)
        {
			_mapper = mapper;
			_loginService= loginService;
		}
        public IActionResult Login()
		{
			SignInViewModel model = new SignInViewModel();
			if (HttpContext.Request.Cookies.TryGetValue("username", out var username))
			{
				model = new SignInViewModel
				{
					Username = username,					
					IsRemember = true
				};
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Login(SignInViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			// TODO: Validate user from database
			var _users = await _loginService.GetUser(model.Username,model.Password);
			if (_users!=null)
			{
				UsersViewModel usersmodel = _mapper.Map<UsersViewModel>(_users);
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, model.Username),
					new Claim(ClaimTypes.Role, "Admin")
				};
				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var authProperties = new AuthenticationProperties
				{
					IsPersistent = model.IsRemember, // This enables "Remember Me"
					ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
				};
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
				if (model.IsRemember)
				{
					var options = new CookieOptions
					{
						Expires = DateTime.UtcNow.AddDays(7), // Set expiry for 7 days						
						SameSite = SameSiteMode.Strict // Prevent CSRF
					};
					HttpContext.Response.Cookies.Append("username", model.Username, options);
				}
				else				
					HttpContext.Response.Cookies.Delete("username");					
				
				return RedirectToAction("Index", "Home");
			}			
			return View(model);
		}
		
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "Login");
		}
	}
}
