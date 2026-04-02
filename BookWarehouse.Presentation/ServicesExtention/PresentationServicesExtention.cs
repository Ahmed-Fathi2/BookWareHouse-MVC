using BookWarehouse.Application.Comman.Settings;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using System.Configuration;

namespace BookWarehouse.Presentation.ServicesExtention
{
    public static class PresentationServicesExtention
    {

        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
