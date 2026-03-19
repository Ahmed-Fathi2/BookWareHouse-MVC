using BookWarehouse.Application.Comman;
using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Application.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Abstractions
{
    public interface IProductService
    {
        Task<Result<IEnumerable<ProductReadVM>>> GetAllProducts();
        Task<Result<ProductReadDetailsVM>> GetProductById(Guid id);
        Task<Result> CreateProduct(ProductCreateVM productCreateVM);

        Task<Result> UpdateProduct(ProductReadVM productReadVM);

        Task<Result> DeleteProduct(Guid id);
    }
}
