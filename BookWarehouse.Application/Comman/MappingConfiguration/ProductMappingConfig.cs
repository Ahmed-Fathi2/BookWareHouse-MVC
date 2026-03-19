using BookWarehouse.Application.ViewModels.Product;
using BookWarehouse.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Comman.MappingConfiguration
{
    public class ProductMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
          config.NewConfig<Product,ProductReadVM>()
                .Map(dest => dest.CategoryName, src => src.Category.Name);

            config.NewConfig<Product, ProductReadDetailsVM>()
              .Map(dest => dest.CategoryName, src => src.Category.Name);
        }
    }
}
