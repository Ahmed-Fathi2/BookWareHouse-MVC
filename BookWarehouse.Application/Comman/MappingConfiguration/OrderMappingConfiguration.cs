using BookWarehouse.Application.ViewModels.Order;
using BookWarehouse.Domain.Entities;
using Mapster;

namespace BookWarehouse.Application.Comman.MappingConfiguration
{
    public class OrderMappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            config.NewConfig<OrderDetails, OrderItemVM>()
                .Map(dest => dest.Title, src => src.Product.Title)
                 .Map(dest => dest.ImageUrl, src => src.Product.ImageUrl);

            config.NewConfig<Order, OrderDetailsVM>()
                .Map(dest => dest.Items, src => src.OrderDetails)
                .Map(dest => dest.Email, src => src.ApplicationUser.Email);
      

        }
    }
}
