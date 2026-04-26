using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Settings;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using BookWarehouse.Infrastructure.Persistence.Repositories;
using BookWarehouse.Infrastructure.Persistence.Seeders;
using BookWarehouse.Infrastructure.Services.Auth;
using BookWarehouse.Infrastructure.Services.File;
using BookWarehouse.Infrastructure.Services.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace BookWarehouse.Infrastructure.ServicesExtention
{
    public static class InfrastructureServicesExtension
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IDataBaseSeeder, DataBaseSeeder>();
            services.AddScoped<ICategorySeeder, CategorySeeder>();
            services.AddScoped<IRoleSeeder, RoleSeeder>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();
            //services.AddScoped<IPaymentService, StripePaymentService>();
         
            services.AddScoped<IPaymentService, KashierPaymentService>();
            services.AddHttpClient<IPaymentService, KashierPaymentService>();


            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Configure<UploadImageSetting>
                (configuration.GetSection(nameof(UploadImageSetting)));

            services.Configure<StripeSetting>
                (configuration.GetSection(nameof(Stripe)));

            var Kashier = "Kashier";

            services.Configure<KashierSettings>
                (configuration.GetSection(nameof(Kashier)));



            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddSignInManager();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"] ?? throw new InvalidOperationException("Google ClientId not found.");
                    options.ClientSecret = googleAuthNSection["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not found.");
                });


            services.Configure<IdentityOptions>(options =>
             {

                 // Default SignIn settings.
                 options.SignIn.RequireConfirmedEmail = true;
                 options.SignIn.RequireConfirmedPhoneNumber = false;

                 //Password settings.
                 options.Password.RequireDigit = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireUppercase = false;
                 options.Password.RequiredLength = 8;
                 options.Password.RequiredUniqueChars = 0;



                 // Lockout settings.
                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                 options.Lockout.MaxFailedAccessAttempts = 3;
                 options.Lockout.AllowedForNewUsers = true;

                 // User settings.
                 options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                 options.User.RequireUniqueEmail = true;  /*********************************************************************/
             });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = "BookWarehouse.Auth";
                options.Cookie.HttpOnly = true; // Mitigate XSS attacks by preventing client-side scripts from accessing the cookie
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are only sent over HTTPS
                //options.Cookie.SameSite = SameSiteMode.Lax; //** Prevent the browser from sending the cookie along with cross-site requests, mitigating CSRF attacks
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                // Expiration
                options.ExpireTimeSpan = TimeSpan.FromDays(7); // Set a reasonable expiration time for the authentication cookie
                options.SlidingExpiration = true; // Renew the cookie on each request to keep the user logged in

                // Redirects
                options.LoginPath = "/Auth/Login"; // Redirect to the login page when an unauthenticated user tries to access a protected resource
                options.LogoutPath = "/Auth/Logout"; // Redirect to the logout page when the user logs out
                options.AccessDeniedPath = "/Auth/AccessDenied"; // Redirect to an access denied page when the user tries to access a resource they don't have permission for
            });

            //options.Cookie.SameSite = SameSiteMode.None; // 🔥 مهم
            //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // لازم HTTPS

            return services;
        }
    }
}
