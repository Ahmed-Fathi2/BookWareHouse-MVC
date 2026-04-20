using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.Comman.Settings;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Payment;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BookWarehouse.Infrastructure.Services.Payment
{
    public class KashierPaymentService(
        IUnitOfWork unitOfWork,
        IOptions<KashierSettings> options,
        HttpClient httpClient,
        ILogger<KashierPaymentService> logger,
        ICartService cartService
             ) : IPaymentService

    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<KashierPaymentService> _logger = logger;
        private readonly ICartService _cartService = cartService;
        private readonly KashierSettings _options = options.Value;


        public async Task<Result<string>> CreateCheckoutSessionAsync(string origin, int orderId, IEnumerable<CartDetailsVM>? cartDetailsVMs)
        {

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order is null)
                return Result.Failure<string>(
                    new Error("Order.NotFound", $"Order with id {orderId} not found"));

            var requestBody = new
            {
                expireAt = DateTime.UtcNow.AddMinutes(30).ToString("o"),
                maxFailureAttempts = 3,
                paymentType = "credit",

                amount = order.OrderTotal.ToString("F2"), //  "100.00"
                currency = "EGP",

                order = order.Id.ToString(),
                merchantId = _options.MerchantId,

                //callback URL for redirection after payment  
                merchantRedirect = $"{origin}/payment/callback",
                //merchantRedirect = $"{origin}/Cart/OrderConfirmation?orderId ={orderId}",
                // Webhook URL for Kashier to send payment status updates
                serverWebhook = $"{origin}/api/payment/webhook",

                description = $"Payment for Order {order.Id}",

                allowedMethods = "card,wallet",
                type = "one-time",
                display = "ar",

                customer = new
                {
                    email = "test@test.com",
                    reference = order.ApplicationUserId
                }
            };


            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var request = new HttpRequestMessage(
                HttpMethod.Post, _options.BaseUrl);

            request.Headers.Add("Authorization", _options.Secret); // secret key
            request.Headers.Add("api-key", _options.ApiKey);       // public api key

            request.Content = content;


            var response = await _httpClient.SendAsync(request);


            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return Result.Failure<string>(
                    new Error("Kashier.ApiError", $"Kashier API returned status code {response.StatusCode} with body: {responseBody}"));


            using var doc = JsonDocument.Parse(responseBody);


            if (!doc.RootElement.TryGetProperty("sessionUrl", out var sessionUrlElement))
                return Result.Failure<string>(
                    new Error("Kashier.InvalidResponse", "Kashier response is missing sessionUrl"));


            var sessionUrl = sessionUrlElement.GetString();

            if (string.IsNullOrEmpty(sessionUrl))
                return Result.Failure<string>(
                    new Error("Kashier.InvalidResponse", "Kashier response contains empty sessionUrl"));


            //  sessionId 
            if (!doc.RootElement.TryGetProperty("_id", out var sessionIdElement))
                return Result.Failure<string>(
                    new Error("Kashier.InvalidResponse", "Missing sessionId"));

            var sessionId = sessionIdElement.GetString();

            if (string.IsNullOrEmpty(sessionUrl) || string.IsNullOrEmpty(sessionId))
                return Result.Failure<string>(
                    new Error("Kashier.InvalidResponse", "Invalid session data"));

            order.SessionId = sessionId;
            await _unitOfWork.SaveChangesAsync();


            return Result.Success(sessionUrl!);
        }

    
        public async Task<Result<WebHookVM>> HandleWebhookAsync(string body, string signature)
        {
            if (!ValidateSignature(body, signature, _options.ApiKey))
            {
                _logger.LogWarning("Invalid webhook signature. Received signature: {ReceivedSignature}", signature);
                return Result.Failure<WebHookVM>(new Error("Kashier.InvalidSignature", "Invalid webhook signature")); 
            }


            var json = JsonDocument.Parse(body);
            var data = json.RootElement.GetProperty("data");

            var orderId = data.GetProperty("merchantOrderId").GetString();
            var status = data.GetProperty("status").GetString();
            var transactionId = data.GetProperty("transactionId").GetString(); // this id present payment process that happen

            var webHookVM = new WebHookVM
            {
                OrderId = int.Parse(orderId!),
                Status = status!,
                TransactionId = transactionId!
            };


            return Result.Success(webHookVM);

        }

        public async Task<Result> RefundPaymentAsync(int orderId,string transactionId ,decimal amount)
        {
            var  order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if(order is null)
                return Result.Failure(new Error("Order.NotFound", $"Order with id {orderId} not found"));


            var url = $"https://test-fep.kashier.io/v3/orders/{orderId}";

            var body = new
                {
                    apiOperation = "REFUND",
                    reason = "Customer request",
                    transaction = new
                    {
                        //amount = amount.ToString("F2"), // "100.00"
     
                        amount // "100.00"

                    }
                };

                var json = JsonSerializer.Serialize(body);

                var request = new HttpRequestMessage(HttpMethod.Put, url);

                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                request.Headers.Add("Authorization",_options.Secret);
                request.Headers.Add("accept", "application/json");

            var response = await httpClient.SendAsync(request);

                var responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (!response.IsSuccessStatusCode)
                {
                    // Graceful handling if the order was already refunded
                    var cause = root.TryGetProperty("error", out var errorEl) && errorEl.TryGetProperty("cause", out var causeEl) 
                        ? causeEl.GetString() 
                        : null;

                    if (cause == "fully refunded order")
                    {
                        return Result.Success();
                    }

                    return Result.Failure(
                        new Error("Kashier.ApiError", responseBody));
                }
                  


                var status = root.GetProperty("status").GetString();

                if (status != "SUCCESS")
                {
                    var message = root.GetProperty("messages")
                                      .GetProperty("en")
                                      .GetString();

                    return Result.Failure(
                        new Error("Kashier.RefundFailed", message ?? "Refund failed"));
                }

          
                return Result.Success();
            
          
        }

        private bool ValidateSignature(string body, string receivedSignature, string secret)
        {
            var json = JsonDocument.Parse(body);
            var data = json.RootElement.GetProperty("data");

            var keys = data.GetProperty("signatureKeys")
                           .EnumerateArray()
                           .Select(x => x.GetString())
                           .Where(x => x != null)
                           .OrderBy(x => x)
                           .ToList();


            // payload format: amount=250&currency=EGP&status=SUCCESS  make a string from keys and values (sorted by keys)
            var sb = new StringBuilder();

            foreach (var key in keys)
            {
                var value = data.GetProperty(key!).ToString();


                // Hash has _&| so use Uri.EscapeDataString to encode these chars in value (not key)
                var encodedValue = Uri.EscapeDataString(value);

                sb.Append($"{key}={encodedValue}&");
            }

            var payload = sb.ToString().TrimEnd('&');


            // HMACSHA256 :ALgorithm for hashing with secret key 
            // hash →  payload + secret
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            var computedSignature = BitConverter.ToString(hash)
                .Replace("-", "")
                .ToLower();

            return computedSignature == receivedSignature;
        }
    }
}