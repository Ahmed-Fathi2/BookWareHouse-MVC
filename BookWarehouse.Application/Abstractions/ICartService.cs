using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Product;

namespace BookWarehouse.Application.Abstractions
{
    public interface ICartService
    {
        Task<Result<IEnumerable<ProductReadDetailsVM>>> GetAllUserCartProducts (string userId);
        Task AddToCart(CreateCartVM createCartVM);

    }
}
