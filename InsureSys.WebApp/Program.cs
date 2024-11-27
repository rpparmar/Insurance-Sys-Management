using InsuranceSys.Application;
using InsuranceSys.Infrastructure;
using InsuranceSys.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using InsuranceSys.Infrastructure.Utility;
using Insurancesys.web.Utility;
using InsuranceSys.Application.Interface;

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
            builder.Services.AddAutoMapper(typeof(ViewModelDtoMapping)); // Scans for profiles in the assembly
            builder.Services.AddAutoMapper(typeof(EntityDtoMapping)); // Scans for profiles in the assembly


            // Build the configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Services.AddScoped<IAppDBContext, AppDBContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("MasterConnection") ?? string.Empty;
                return new AppDBContext(connectionString);
            });

            builder.Services.AddDbContext<EfdbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MasterConnection") ?? string.Empty));

            RegisterDependency(builder);

            #region JWT Authentication for Unauthorized Access
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
            #endregion
            #region Cookie Authentication for Unauthorized Access
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

        private static void RegisterDependency(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<DbContextFactory>(); //used this for allowing dynamic context/connectionstring with EF
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<ICountryService, CountryService>();
        }
    }
}