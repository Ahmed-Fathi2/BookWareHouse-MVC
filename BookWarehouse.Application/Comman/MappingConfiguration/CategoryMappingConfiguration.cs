using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Domain.Entities;
using Mapster;

namespace BookWarehouse.Application.Comman.MappingConfiguration
{
    public class CategoryMappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryReadEditVM>()
                .Map(dest => dest.NumOfProducts, src => src.Products.Count);


        
        }
    }
}
