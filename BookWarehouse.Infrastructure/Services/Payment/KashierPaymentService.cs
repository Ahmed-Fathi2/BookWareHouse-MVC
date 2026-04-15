using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.Comman.Settings;
using BookWarehouse.Domain.Repositories;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace BookWarehouse.Infrastructure.Services.Payment
{
    public class KashierPaymentService : IKashierPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly KashierSettings _options;

        public KashierPaymentService(
            IUnitOfWork unitOfWork,
            IOptions<KashierSettings> options,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<Result<string>> InitiatePaymentAsync(string origin, int orderId)
        {
            // 1️⃣ Get order from DB
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order is null)
                return Result.Failure<string>(
                    new Error("Order.NotFound", $"Order with id {orderId} not found"));

            // 2️⃣ Build request body (Kashier API format)
            var requestBody = new
            {
                expireAt = DateTime.UtcNow.AddMinutes(30).ToString("o"), // session expiration time (ISO 8601 format)
                maxFailureAttempts = 3,
                paymentType = "credit",

                amount = order.OrderTotal.ToString("F2"), // must be string "100.00"
                currency = "EGP",

                order = order.Id.ToString(),// important for tracking
                merchantId = _options.MerchantId,
        

                // 🔥 IMPORTANT: must be public valid URL (NOT localhost)
                merchantRedirect = $"{origin}/payment/callback",

                // 🔥 MOST IMPORTANT: backend webhook
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

            // 3️⃣ Serialize to JSON
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 4️⃣ Create HTTP request (BEST PRACTICE)
            var request = new HttpRequestMessage(
                HttpMethod.Post, _options.BaseUrl);

            request.Headers.Add("Authorization", _options.Secret); // secret key
            request.Headers.Add("api-key", _options.ApiKey);       // public api key

            request.Content = content;

            // 5️⃣ Send request
            var response = await _httpClient.SendAsync(request);

            // 6️⃣ Read response
            var responseBody = await response.Content.ReadAsStringAsync();

            // 7️⃣ Handle failure (VERY IMPORTANT)
            if (!response.IsSuccessStatusCode)
            {
                                throw new Exception($@"
                ================ KASHIER ERROR ================

                Status Code: {response.StatusCode}

                Response Body:
                {responseBody}

                ==============================================
                ");
            }

            // 8️⃣ Parse response
            using var doc = JsonDocument.Parse(responseBody);

            //  MOST IMPORTANT: get sessionUrl for redirection
            if (!doc.RootElement.TryGetProperty("sessionUrl", out var sessionUrlElement))
                throw new Exception("Kashier response missing sessionUrl");

            var sessionUrl = sessionUrlElement.GetString();

            ////
            //order.SessionId = sessionUrl;
            //await _unitOfWork.SaveChangesAsync();
            
            // 9️⃣ Return redirect URL
            return Result.Success(sessionUrl!);
        }
    }
}