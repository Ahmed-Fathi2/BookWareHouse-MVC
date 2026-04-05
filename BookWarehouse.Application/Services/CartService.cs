using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Product;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Mapster;

namespace BookWarehouse.Application.Services
{
    public class CartService(IUnitOfWork unitOfWork) : ICartService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        public async Task<Result<IEnumerable<CartDetailsVM>>> GetAllUserCartProducts(string userId)
        {

            var cartProducts = await _unitOfWork.CartRepository
                                        .GetAllAsync(c => c.ApplicationUserId == userId && !c.IsDeleted,
                                          includes: [c => c.Product]);

            var result = cartProducts.Adapt<IEnumerable<CartDetailsVM>>();

            return Result.Success(result);
        }
        public async Task AddToCart(CreateCartVM createCartVM)
        {
            // if cart already exists, update count

            var existingCart = await _unitOfWork.CartRepository
                                        .GetCartByFilter(c => c.ApplicationUserId == createCartVM.ApplicationUserId
                                                      && c.ProductId == createCartVM.ProductId
                                                      && !c.IsDeleted);

            if (existingCart is not null)
            {
                existingCart.Count = createCartVM.Count;
            }
            else
            {
                var cart = createCartVM.Adapt<Cart>();
                _unitOfWork.CartRepository.Add(cart);
            }

            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<Result> IncreaseQuantity(Guid cartId)
        {
            var cart = await _unitOfWork.CartRepository.GetByIdAsync(cartId);

            if (cart is null)
                return Result.Failure(new Error("Cart not found", "Cart Not Found"));


            cart.Count++;
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DecreaseQuantity(Guid cartId)
        {
            var cart = await _unitOfWork.CartRepository.GetByIdAsync(cartId);

            if (cart is null)
                return Result.Failure(new Error("Cart not found", "Cart Not Found"));

            if (cart.Count <= 1)
                return Result.Success();

            cart.Count--;
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();

        }

        public async Task<Result> DeleteFromCart(Guid cartId)
        {
            var cart = await _unitOfWork.CartRepository.GetByIdAsync(cartId);

            if (cart is null)
                return Result.Failure(new Error("Cart not found", "Cart Not Found"));

            _unitOfWork.CartRepository.Delete(cart);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();

        }
    }
}
