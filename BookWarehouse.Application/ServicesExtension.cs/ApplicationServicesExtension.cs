using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman;
using BookWarehouse.Application.Comman.Settings;
using BookWarehouse.Application.Services;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.ServicesExtension.cs
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();

            // Mapster
            services.AddMapster();
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(AssemblyMarker).Assembly);


        

            return services;
        }
    }
}
