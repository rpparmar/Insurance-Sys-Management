using Insurancesys.web.Utility;
using InsuranceSys.Application;
using InsuranceSys.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

namespace Insurancesys.web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(); // Optional - Add services to the container - used to have cshtml changes runtime.
            builder.Services.AddControllers();

            // Add session services
            builder.Services.AddDistributedMemoryCache(); // Registers a default in-memory cache implementation.
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout.
                options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only for security.
                options.Cookie.IsEssential = true; // Ensure the session cookie is essential.
            });

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile)); // Scans for profiles in the assembly


            // Build the configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();


            builder.Services.AddSingleton<IAppDBContext, AppDBContext>(provider =>
    new AppDBContext(configuration.GetConnectionString("MasterConnection") ?? string.Empty));
            
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();

            #region Cookie Authentication for Unauthorized Access

            // Configure JWT authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Set to true if you have an issuer to validate
                    ValidateAudience = false, // Set to true if you have an audience to validate
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"))
                };
            });

            // Configure cookie authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "YourCookieName";
                    options.LoginPath = $"/"; // Customize the login path as needed
                    //options.Events = new CookieAuthenticationEvents
                    //{
                    //    OnRedirectToLogin = ctx =>
                    //    {
                    //        return Task.FromResult<object>(null);
                    //    }
                    //};
                });
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
            app.Run();
        }
    }
}