using Insurancesys.web.Middleware;
using Insurancesys.web.Services;
using Insurancesys.web.Utility;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using InsuranceSys.Infrastructure.Repositories;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
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

            // Development-only: optional LocalMasterConnection overrides ConnectionStrings:MasterConnection for the whole host
            // (Serilog, EF MasterDbContext, repositories) without renaming keys or touching Infrastructure.
            if (builder.Environment.IsDevelopment())
            {
                var localMaster = builder.Configuration.GetConnectionString("LocalMasterConnection");
                if (!string.IsNullOrWhiteSpace(localMaster))
                {
                    builder.Configuration.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:MasterConnection"] = localMaster.Trim()
                        });
                }
            }

            #region Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer(
                    connectionString: builder.Configuration.GetConnectionString("MasterConnection"),
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "AppLogs",
                        AutoCreateSqlTable = true
                    },
                    restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();

            builder.Host.UseSerilog();
            #endregion

            builder.Services
                .AddControllersWithViews(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.ReturnHttpNotAcceptable = true;
                })
                .AddXmlSerializerFormatters();
            //.AddRazorRuntimeCompilation(); // Optional - allows cshtml runtime updates (dev)

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

            #region Authentication — Cookie (default for MVC) + JWT Bearer (for API controllers)
            var jwtSecret = builder.Configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "InsureSysManagement";
            var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "InsureSysManagement";

            builder.Services.AddAuthentication(options =>
            {
                // Cookie is the default for all MVC/Razor actions that use [Authorize] without a scheme.
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "CookieAuth";
                options.LoginPath = "/";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            })
            .AddJwtBearer(options =>
            {
                // JWT Bearer is used only by API controllers that explicitly declare
                // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)].
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion

            #region CORS — allow Angular dev server and production origin
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            #endregion

            #region API versioning
            builder.Services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);

                    options.AssumeDefaultVersionWhenUnspecified = true;

                    options.ReportApiVersions = true;

                    options.ApiVersionReader =
                        new UrlSegmentApiVersionReader();
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
            app.UseCors("AngularPolicy");
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Login}/{id?}");

            await app.RunAsync();
        }

        private static void RegisterDependency(WebApplicationBuilder builder)
        {
            // JWT token generation service (singleton — stateless, uses IConfiguration)
            builder.Services.AddSingleton<JwtService>();

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
            builder.Services.AddScoped<ISubUsersService, SubUsersRepository>();

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