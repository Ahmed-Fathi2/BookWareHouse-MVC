using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;

namespace BookWarehouse.Application.Abstractions
{
    public interface ICartService
    {
        Task<Result<IEnumerable<CartDetailsVM>>> GetAllUserCartProducts(string userId);
        Task AddToCart(CreateCartVM createCartVM);

        Task<Result> DeleteFromCart(int cartId);

        Task<Result> IncreaseQuantity(int cartId);

        Task<Result> DecreaseQuantity(int cartId);

        Task<Result> ClearCart(int orderId);
    }
}
