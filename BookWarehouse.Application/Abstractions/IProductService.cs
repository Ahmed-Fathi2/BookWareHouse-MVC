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
        Task<Result<IEnumerable<ProductReadVM>>> GetAllProducts();
        Task<Result<ProductReadDetailsVM>> GetProductById(Guid id);
        Task<Result<ProductEditVM>> GetProductForEdit(Guid id);
        Task<Result> CreateProduct(ProductCreateVM productCreateVM);

        Task<Result> UpdateProduct(ProductEditVM productEditVM, string webRootPath, string? newImageName, Stream? imageStream);

        Task<Result> DeleteProduct(Guid id, string webRootPath);
    }
}
