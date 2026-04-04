using Insurancesys.web.Middleware;
using Insurancesys.web.Utility;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using InsuranceSys.Infrastructure.Repositories;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Text;

namespace Insurancesys.web
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer(
                    connectionString: builder.Configuration.GetConnectionString("MasterConnection"),
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "AppLogs",
                        AutoCreateSqlTable = true
                    },
                    restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();

            builder.Host.UseSerilog();
            #endregion

            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(); // Optional - Add services to the container - used to have cshtml changes runtime.
            builder.Services.AddControllers();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout.
                options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only for security.
                options.Cookie.IsEssential = true; // Ensure the session cookie is essential.
            });

            // Add AutoMapper
            builder.Services.AddAutoMapper(_ => { }, typeof(ViewModelDtoMapping).Assembly); // Scans for profiles in the assembly           

            #region Database Contexts
            builder.Services.AddDbContext<MasterDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("MasterConnection") ?? string.Empty,
                    sql => sql.EnableRetryOnFailure(3)));
            #endregion

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
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "CookieAuth";
                    options.LoginPath = "/";
                    options.AccessDeniedPath = "/Login/Login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                    //options.Events = new CookieAuthenticationEvents
                    //{
                    //    OnRedirectToLogin = ctx =>
                    //    {
                    //        return Task.FromResult<object>(null);
                    //    }
                    //};
                });
            #endregion

            #region Initialize Encryption
            var encryptionKey = builder.Configuration["Encryption:MasterKey"];
            if (!string.IsNullOrEmpty(encryptionKey))
                Cryptography.Initialize(encryptionKey);
            #endregion

            var app = builder.Build();

            #region Seed Master DB
            using (var scope = app.Services.CreateScope())
            {
                var masterDb = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
                await masterDb.Database.EnsureCreatedAsync();
                await masterDb.SeedSuperAdminAsync();
            }
            #endregion

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
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Login}/{id?}");

            await app.RunAsync();
        }

        private static void RegisterDependency(WebApplicationBuilder builder)
        {
            // Tenant infrastructure (Singleton for cache, Scoped for per-request components)
            builder.Services.AddSingleton<ITenantConnectionCache, TenantConnectionCache>();
            builder.Services.AddScoped<ITenantConnectionResolver, TenantConnectionResolver>();

            // Connection providers (Scoped - one per request, tenant-aware)
            builder.Services.AddScoped<IEFdbContextProvider, EFdbContextProvider>();
            builder.Services.AddScoped<ISqlConnectionProvider, SqlConnectionProvider>();
            builder.Services.AddScoped<IAdoNetDBContext, AdoNetDBContext>();

            // Master DB services
            builder.Services.AddScoped<IMasterLoginService, MasterLoginRepository>();
            builder.Services.AddScoped<IAgencyOnboardingService, AgencyOnboardingRepository>();

            // Existing tenant-scoped services
            builder.Services.AddScoped<ICompanyService, CompanyRepository>();
            builder.Services.AddScoped<ILeadService, LeadRepository>();
            builder.Services.AddScoped<ILoginService, LoginRepository>();
            builder.Services.AddScoped<IInsuranceTypeService, InsuranceTypeRepository>();
            builder.Services.AddScoped<IDropDownBinderService, DropDownBinderRepository>();
            builder.Services.AddScoped<ILeadStatusService, LeadStatusRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerRepository>();
            builder.Services.AddScoped<ICountryService, CountryRepository>();
            builder.Services.AddScoped<IStateService, StateRepository>();

        }
    }
}