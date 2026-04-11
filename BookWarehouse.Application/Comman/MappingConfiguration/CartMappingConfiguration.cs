using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Comman.MappingConfiguration
{
    public class CartMappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Cart, CartDetailsVM>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Product.Title)
                .Map(dest => dest.Description, src => src.Product.Description)
                .Map(dest => dest.Author, src => src.Product.Author)
                .Map(dest => dest.ListPrice, src => src.Product.ListPrice)
                .Map(dest => dest.Price, src => src.Product.Price)
                .Map(dest => dest.Price50, src => src.Product.Price50)
                .Map(dest => dest.Price100, src => src.Product.Price100)
                .Map(dest => dest.ImageUrl, src => src.Product.ImageUrl)
                .Map(dest => dest.Count, src => src.Count)
                .Map(dest=> dest.ProductId, src => src.ProductId)
                .Map(dest => dest.FinalPrice,
                          src =>
                          src.Count <= 50
                        ? src.Product.Price * src.Count
                        : (src.Count <= 100
                            ? src.Product.Price50 * src.Count
                            : src.Product.Price100 * src.Count));

        }
    }
}
