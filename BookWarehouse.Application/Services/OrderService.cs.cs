using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Mapster;

namespace BookWarehouse.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork , ICartService cartService) : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICartService _cartService = cartService;

        public async Task<Result> PlaceOrderAsync(CheckoutVM checkoutVM)
        {

        
            var order = checkoutVM.Adapt<Order>();
            _unitOfWork.OrderRepository.Add(order);

            await _unitOfWork.SaveChangesAsync();

            var cartItemsResult = await _cartService.GetAllUserCartProducts(checkoutVM.ApplicationUserId);


            var orderDetailsList = cartItemsResult.Value.Select(item => new OrderDetails
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Count,
                Price = item.FinalPrice/ item.Count 

            }).ToList();


            _unitOfWork.OrderDetailsRepository.AddRange(orderDetailsList);

            await _unitOfWork.SaveChangesAsync();  

            return Result.Success();


        }
    }
}




      //,[OrderDate] //Done
      //,[OrderStatus]//Done
      //,[PaymentStatus] // Done

      //,[PaymentDate] // afetr successful payment, set this to current date
      //,[SessionId] // Stripe session id for the payment, can be used to verify payment status with Stripe API
      //,[PaymentIntentId] // Stripe payment intent id, can be used to verify payment status with Stripe API

      //// Done
      //,[OrderTotal]
      //,[FullName]
      //,[PhoneNumber]
      //,[StreetAddress]
      //,[City]
      //,[ApplicationUserId]


      //// OrderDetails
      //,[OrderId]
      //,[ProductId]
      //,[Quantity]
      //,[Price] // Price of one unit  at the time of order, not the current product price
