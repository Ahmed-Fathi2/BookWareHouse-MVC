using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Application.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Abstractions
{
    public interface IProductService
    {
        Task<Result<IEnumerable<ProductReadVM>>> GetAllProducts(int? categoryId = null);
        Task<Result<ProductReadDetailsVM>> GetProductById(int id);
        Task<Result<ProductEditVM>> GetProductForEdit(int id);
        Task<Result> CreateProduct(ProductCreateVM productCreateVM);

        Task<Result> UpdateProduct(ProductEditVM productEditVM, string webRootPath, string? newImageName, Stream? imageStream);

        Task<Result> DeleteProduct(int id, string webRootPath);
    }
}
