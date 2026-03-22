using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Settings;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using BookWarehouse.Infrastructure.Persistence.Repositories;
using BookWarehouse.Infrastructure.Persistence.Seeders;
using BookWarehouse.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


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
            services.AddScoped<IFileService, FileService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

           // services.AddOptions<UploadImageSetting>()
           //.Bind(configuration.GetSection(nameof(UploadImageSetting)))
           //.ValidateDataAnnotations()
           //.ValidateOnStart();

            return services;
        }
    }
}
