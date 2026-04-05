using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Product;

namespace BookWarehouse.Application.Abstractions
{
    public interface ICartService
    {
        Task<Result<IEnumerable<CartDetailsVM>>> GetAllUserCartProducts (string userId);
        Task AddToCart(CreateCartVM createCartVM);

        Task<Result> DeleteFromCart(Guid cartId);

        Task<Result> IncreaseQuantity(Guid cartId);

        Task<Result> DecreaseQuantity(Guid cartId);

    }
}
