using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Order;
using BookWarehouse.Application.ViewModels.Payment;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BookWarehouse.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork,
        ICartService cartService,
        IPaymentService PaymentService,
        ILogger<OrderService> logger
    ) : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICartService _cartService = cartService;
        private readonly IPaymentService _paymentService = PaymentService;
        private readonly ILogger<OrderService> _logger = logger;

        public async Task<Result<string>> PlaceOrderAsync(string origin, CheckoutVM checkoutVM)
        {
            var cartItemsResult = await _cartService.GetAllUserCartProducts(checkoutVM.ApplicationUserId);

            if (!cartItemsResult.IsSuccess || !cartItemsResult.Value.Any())
                return Result.Failure<string>(new Error("Cart is empty", "Cart.Empty"));

            var cartItems = cartItemsResult.Value;

            Order order;

            // DB Transaction (ONLY DB)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                order = await GetOrCreateOrderAsync(checkoutVM, cartItems);

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<string>(new Error(ex.Message, "Order.Failed"));
            }

            // Create Payment Session (External API Call)
            var sessionResult = await _paymentService.CreateCheckoutSessionAsync(origin, order.Id, cartItems);


            //var sessionResult = await _kashierPaymentService.InitiatePaymentAsync(origin, order.Id);

            if (!sessionResult.IsSuccess)
                return Result.Failure<string>(sessionResult.Error);

            return Result.Success(sessionResult.Value);
        }

        public async Task<Result<IEnumerable<OrderDetailsVM>>> GetAllOrdersAsync(OrderStatus? orderStatus,string? userId, string? searchValue)
        {
            Expression<Func<Order, bool>>? filter = o =>
                (!orderStatus.HasValue || o.OrderStatus == orderStatus.Value) &&
                (string.IsNullOrEmpty(userId) || o.ApplicationUserId == userId) &&
                (string.IsNullOrEmpty(searchValue) ||( o.Id.ToString().Contains(searchValue)));

            var orders = await _unitOfWork.OrderRepository.GetAllOrders(filter: filter);

            var orderReadVMs = orders.Adapt<IEnumerable<OrderDetailsVM>>();



            return Result.Success(orderReadVMs);
        }

        public async Task<Result<OrderDetailsVM>> GetOrderDeatilsByIdAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderDetails(orderId);
            if (order == null)
                return Result.Failure<OrderDetailsVM>(new Error("Order not found", "Order.NotFound"));

            var result = order.Adapt<OrderDetailsVM>();

            return Result.Success(result);
        }

        public async Task<Result> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result.Failure(new Error("Order not found", "Order.NotFound"));


            if (status == OrderStatus.Shipped)
                order.ShippingDate = DateTime.UtcNow;


            order.OrderStatus = status;

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();

        }

        public async Task<Result> UpdateOrderDetailsAsync(int orderId, string carrier, string trackingNumber)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result.Failure(new Error("Order not found", "Order.NotFound"));

            order.Carrier = carrier;
            order.TrackingNumber = trackingNumber;


            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task HandlePaymentResult(WebHookVM webHookVM)
        {

            var order = await _unitOfWork.OrderRepository.GetOrderById(webHookVM.MerchantOrderId);
            if (order == null)
            {
                _logger.LogWarning("Received webhook for non-existent order with merchant order id {MerchantOrderId}", webHookVM.MerchantOrderId);
                return;
            }

            if (string.Equals(webHookVM.Status, PaymentWebhookStatus.SUCCESS.ToString(), StringComparison.OrdinalIgnoreCase))
                await UpdateCompletedOrder(webHookVM.Status, webHookVM.TransactionId, order!);

            if (string.Equals(webHookVM.Status, PaymentWebhookStatus.FAILURE.ToString(), StringComparison.OrdinalIgnoreCase))
                await UpdateFailedOrder(webHookVM.Status, webHookVM.TransactionId, order!);

        }


        #region Private Methods

        private async Task<Order> GetOrCreateOrderAsync(CheckoutVM checkoutVM, IEnumerable<CartDetailsVM> cartItems)
        {
            var cartSignature = GenerateCartSignature(cartItems);

            var existingOrders = await _unitOfWork.OrderRepository
                .GetAllAsync(o =>
                    o.ApplicationUserId == checkoutVM.ApplicationUserId && (o.OrderStatus == OrderStatus.Pending),
                    tracked: true);

            var order = existingOrders
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefault();

            //1- If no existing order, create new
            if (order == null)
                return await CreateNewOrder(checkoutVM, cartItems, cartSignature);

            //2- If order exists and cart signature matches, reuse it
            if (order.CartSignature == cartSignature)
            {
                order.OrderStatus = OrderStatus.Pending;
                order.PaymentStatus = PaymentStatus.Pending;
                order.SessionId = null;
                order.PaymentIntentId = null;

                await _unitOfWork.SaveChangesAsync();
                return order;
            }

            //3- If order exists but cart has changed, cancel old order and create new one
            order.OrderStatus = OrderStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();
            return await CreateNewOrder(checkoutVM, cartItems, cartSignature);
        }

        private async Task<Order> CreateNewOrder(CheckoutVM checkoutVM, IEnumerable<CartDetailsVM> cartItems, string signature)
        {
            var newOrder = checkoutVM.Adapt<Domain.Entities.Order>();
            newOrder.CartSignature = signature;

            _unitOfWork.OrderRepository.Add(newOrder);

            await _unitOfWork.SaveChangesAsync();


            var orderDetailsList = cartItems.Select(item => new OrderDetails
            {
                OrderId = newOrder.Id,
                ProductId = item.ProductId,
                Quantity = item.Count,
                Price = item.FinalPrice / item.Count // Price of a single item

            }).ToList();


            _unitOfWork.OrderDetailsRepository.AddRange(orderDetailsList);

            await _unitOfWork.SaveChangesAsync();

            return newOrder;
        }

        private async Task UpdateCompletedOrder(string status, string transactionId, Order order)
        {
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                _logger.LogInformation("Order {Id} already marked as Paid. Skipping duplicate webhook event.", order.Id);
                return;
            }

            if (order.OrderStatus == OrderStatus.Cancelled || order.PaymentStatus == PaymentStatus.Refunded)
            {
                _logger.LogInformation("Order {Id} is already cancelled or refunded. Skipping delayed or refund webhook.", order.Id);
                return;
            }

            order.PaymentIntentId = transactionId;
            order.PaymentDate = DateTime.UtcNow;
            order.PaymentStatus = PaymentStatus.Paid;
            order.OrderStatus = OrderStatus.Approved;

            await _cartService.ClearCart(order.Id);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Order {Id} approved", order.Id);

        }

        private async Task UpdateFailedOrder(string status, string transactionId, Order order)
        {

            if (order.PaymentStatus == PaymentStatus.Failed)
            {
                _logger.LogInformation("Order {Id} already marked as failed, skipping", order.Id);
                return;
            }

            order.PaymentIntentId = transactionId;
            order.PaymentStatus = PaymentStatus.Failed;
            order.OrderStatus = OrderStatus.Pending;

            await _unitOfWork.SaveChangesAsync();

            _logger.LogWarning("Payment failed for Order {Id}", order.Id);
        }
        private string GenerateCartSignature(IEnumerable<CartDetailsVM> cartItems)
        {
            return string.Join("|", cartItems
                .OrderBy(x => x.ProductId)
                .Select(x => $"{x.ProductId}-{x.Count}"));
        }

        #endregion

        public async Task<Result> CancelOrderAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result.Failure(new Error("Order not found", "Order.NotFound"));

            if (order.OrderStatus == OrderStatus.Shipped || order.OrderStatus == OrderStatus.Delivered)
                return Result.Failure(new Error("Cannot cancel shipped or delivered order", "Order.CancelFailed"));

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                var refundResult = await _paymentService.RefundPaymentAsync(order.MerchantOrderId, order.PaymentIntentId!, order.OrderTotal);
                if (!refundResult.IsSuccess)
                    return Result.Failure(new Error("Refund failed: " + refundResult.Error.Description, "Order.RefundFailed"));

                order.PaymentStatus = PaymentStatus.Refunded;
                order.OrderStatus = OrderStatus.Cancelled;
            }
            else
            {
                order.PaymentStatus = PaymentStatus.Cancelled;
                order.OrderStatus = OrderStatus.Cancelled;

            }
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();



        }
    }
}


