using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Product;
using BookWarehouse.Domain.Common.Enums;
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

        public async Task<Result> IncreaseQuantity(int cartId)
        {
            var cart = await _unitOfWork.CartRepository.GetByIdAsync(cartId);

            if (cart is null)
                return Result.Failure(new Error("Cart not found", "Cart Not Found"));


            cart.Count++;
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DecreaseQuantity(int cartId)
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

        public async Task<Result> DeleteFromCart(int cartId)
        {
            var cart = await _unitOfWork.CartRepository.GetByIdAsync(cartId);

            if (cart is null)
                return Result.Failure(new Error("Cart not found", "Cart Not Found"));

            _unitOfWork.CartRepository.Delete(cart);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();

        }

        public async Task<Result> ClearCart(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

            if (order is null)
                return Result.Failure(new Error("Order not found", "Order.NotFound"));

            if (order.PaymentStatus != PaymentStatus.Paid || order.OrderStatus != OrderStatus.Approved)
                return Result.Success();

            var carts = await _unitOfWork.CartRepository
                .GetAllAsync(c => c.ApplicationUserId == order.ApplicationUserId && !c.IsDeleted);

            if (!carts.Any())
                return Result.Success();

            _unitOfWork.CartRepository.DeleteAll(carts);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }


    }
}
