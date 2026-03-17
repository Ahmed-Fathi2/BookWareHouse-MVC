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
            // Register application services here
            // e.g., services.AddScoped<IBookService, BookService>();
            return services;
        }
    }
}
