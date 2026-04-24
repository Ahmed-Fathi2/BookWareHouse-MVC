using BookWarehouse.Application.Comman.Pagination;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Product;

namespace BookWarehouse.Application.Abstractions
{
    public interface IProductService
    {
        Task<Result<PagedResult<ProductReadVM>>> GetAllProducts(ProductQueryVM productQueryVM);
        Task<Result<IEnumerable<ProductReadVM>>> GetAllProductsForJson();
        Task<Result<ProductReadDetailsVM>> GetProductById(int id);
        Task<Result<ProductEditVM>> GetProductForEdit(int id);
        Task<Result> CreateProduct(ProductCreateVM productCreateVM);

        Task<Result> UpdateProduct(ProductEditVM productEditVM, string webRootPath, string? newImageName, Stream? imageStream);

        Task<Result> DeleteProduct(int id, string webRootPath);
    }
}
