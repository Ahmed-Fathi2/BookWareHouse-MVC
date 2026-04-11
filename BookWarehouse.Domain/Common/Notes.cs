using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Common
{
    public class Notes
    {
        //asp-validation-summary="ModelOnly"
        //<div asp-validation-summary="ModelOnly" class="text-danger"></div>  --->> show error doesnt related to any prop only
        //like : ModelState.AddModelError("", "Category name cannot be 'test'.");


        //asp-validation-summary="All"
        //<div asp-validation-summary="All" class="text-danger"></div>  --->> show error  all  related to any prop and not related
        //like : ModelState.AddModelError("", "Category name cannot be 'test'.");
        //like : ModelState.AddModelError("name", "Category name cannot be 'test'.");


        //public async Task<Result> PlaceOrderAsync(CheckoutVM checkoutVM)
        //{

        //    // i must use transaction here to ensure that both order and order details are saved successfully,
        //    // //if any of them fails, the transaction will be rolled back

        //    //1- Add order
        //    var order = checkoutVM.Adapt<Order>();
        //    order.ApplicationUserId = checkoutVM.ApplicationUserId;
        //    _unitOfWork.OrderRepository.Add(order);

        //    await _unitOfWork.SaveChangesAsync(); //Save changes to get the generated order id for order details

        //    //var cartItemsResult = await _cartService.GetAllUserCartProducts(checkoutVM.ApplicationUserId);
        //    var cartItemsResult = await _cartService.GetAllUserCartProducts(checkoutVM.ApplicationUserId);

        //    //2- Add order details for each cart item
        //    var orderDetailsList = cartItemsResult.Value.Select(item => new OrderDetails
        //    {
        //        OrderId = order.Id,
        //        ProductId = item.ProductId,
        //        Quantity = item.Count,
        //        Price = item.FinalPrice / item.Count
        //        // For Logging:  price of one unit at the time of order, not the current product price
        //    }).ToList();


        //    _unitOfWork.OrderDetailsRepository.AddRange(orderDetailsList);

        //    await _unitOfWork.SaveChangesAsync();

        //    return Result.Success();


        //}


    }
}
