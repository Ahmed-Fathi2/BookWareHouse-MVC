using BookWarehouse.Application.Abstractions;
using BookWarehouse.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    public class OrderController(IOrderService orderService) : Controller
    {
        private readonly IOrderService _orderService = orderService;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAll(OrderStatus? status)
        {
            var orders = await _orderService.GetAllOrdersAsync(status);

            return Json(new { data = orders.Value });
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDeatilsByIdAsync(id);
            if (!order.IsSuccess)
                return NotFound("Order not found");

            return View(order.Value);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderDetails(int id, string carrier, string trackingNumber)
        {
            var result = await _orderService.UpdateOrderDetailsAsync(id, carrier, trackingNumber);
            if (!result.IsSuccess)
                TempData["error"] = "Failed to update order details.";
            else
                TempData["success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> StartProcessing(int id)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Processing);
            if (!result.IsSuccess)
                TempData["Error"] = "Failed to update order status.";
            else
                TempData["Success"] = "Order is now Processing.";
            return RedirectToAction(nameof(Details), new { id });
        }
      
     

        [HttpPost]
        public async Task<IActionResult> StartShipping(int id, string carrier, string trackingNumber)
        {
            //var req = Request;
            var updateDetailsResult = await _orderService.UpdateOrderDetailsAsync(id, carrier, trackingNumber);
            if (!updateDetailsResult.IsSuccess)
            {
                TempData["Error"] = "Failed to update shipping details.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Shipped);
            if (!result.IsSuccess)
                TempData["Error"] = "Failed to update order status.";
            else
                TempData["Success"] = "Order is now Shipped.";

            return RedirectToAction(nameof(Details), new { id });
        }


        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Cancelled);
            if (!result.IsSuccess)
                TempData["Error"] = "Failed to update order status.";
            else
            {
                // implement refund logic here if necessary
                TempData["Success"] = "Order is now Cancelled.";

            }
            return RedirectToAction(nameof(Details), new { id });
        }

    }
}
