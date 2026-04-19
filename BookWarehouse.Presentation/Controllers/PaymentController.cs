using BookWarehouse.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BookWarehouse.Presentation.Controllers
{
    public class PaymentController : Controller
    {

        // Kashier redirects user here after payment 
        // Success URL: https://yourdomain.com/payment/callback?merchantOrderId=123&paymentStatus=SUCCESS
        [HttpGet("payment/callback")]
        public IActionResult Callback()
        {
            var orderId = Request.Query["merchantOrderId"];
            var status = Request.Query["paymentStatus"];

            Console.WriteLine($"OrderId: {orderId}");
            Console.WriteLine($"Status: {status}");

            // TODO: هنا هتقرأ query params وتعرض النتيجة للمستخدم
            return RedirectToAction("Index", "Home"); // View اسمها Callback.cshtml
        }

        // 🔵 2. Webhook (POST)
        // Kashier calls this endpoint server-to-server
        [HttpPost("api/payment/webhook")]
        public async Task<IActionResult> Webhook()
        {// 1️⃣ Read RAW body (IMPORTANT for signature validation)
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            Console.WriteLine("============================================================");
            Console.WriteLine(body);
            Console.WriteLine("============================================================");

            // 2️⃣ Get signature from header
            var receivedSignature = Request.Headers["x-kashier-signature"].ToString();

            if (string.IsNullOrEmpty(receivedSignature))
                return Unauthorized("Missing signature");

            // 3️⃣ Validate signature
            //if (!ValidateSignature(body, receivedSignature, _secret))
            //    return Unauthorized("Invalid signature");

            if (!ValidateSignature(body, receivedSignature,"5c390a842-b98e-404f-bf96-616e7f9d5403"))
                return Unauthorized("Invalid signature");
            // 4️⃣ Parse JSON AFTER validation
            var json = JsonDocument.Parse(body);
            var data = json.RootElement.GetProperty("data");

            var orderId = data.GetProperty("merchantOrderId").GetString();
            var status = data.GetProperty("status").GetString();
            var transactionId = data.GetProperty("transactionId").GetString(); // Aproximate to PaymentIntitId 

            //// 5️⃣ Get order
            //var order = await _unitOfWork.OrderRepository.GetByIdAsync(int.Parse(orderId!));
            //if (order == null)
            //    return NotFound();

            //// 6️⃣ 🔥 Prevent duplicate processing (IMPORTANT)
            //if (order.PaymentStatus == PaymentStatus.Completed)
            //    return Ok(); // already processed

            // 7️⃣ Update order based on payment status
            //if (status == "SUCCESS")
            //{
            //    order.PaymentStatus = PaymentStatus.Completed;
            //    order.OrderStatus = OrderStatus.Approved;
            //    order.PaymentTransactionId = transactionId;

            //    // 🔥 هنا تمسح الكارت (صح)
            //}
            //else if (status == "FAILED")
            //{
            //    order.PaymentStatus = PaymentStatus.Failed;
            //}
            //else
            //{
            //    order.PaymentStatus = PaymentStatus.Pending;
            //}

            //// 8️⃣ Save changes
            //await _unitOfWork.SaveChangesAsync();

            // 9️⃣ Return 200 OK (VERY IMPORTANT for Kashier)
            return Ok();
        }


        private bool ValidateSignature(string body, string receivedSignature, string secret)
        {
            var json = JsonDocument.Parse(body); // convert string to JSON document
            var data = json.RootElement.GetProperty("data");

            // 1️⃣ Get keys & sort alphabetically
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
/*
 {
  "event": "pay",
  "data": {
    "merchantOrderId": "55",
    "kashierOrderId": "f64a02c6-249e-417a-a6f8-f1cfbb6a4620",
    "orderReference": "TEST-ORD-193450233",
    "transactionId": "TX-4490583927",
    "status": "SUCCESS",
    "method": "card",
    "creationDate": "2026-04-14T23:17:43.654Z",
    "amount": 250,
    "settlementInfo": {
      "vat": "0.00",
      "sellingRate": 0,
      "sellingFlat": 0,
      "totalSellingRate": "0.00",
      "totalSellingFees": "0.00",
      "settledAmount": "250.00"
    },
    "currency": "EGP",
    "card": {
      "cardInfo": {
        "cardHolderName": "fathi",
        "cardBrand": "Visa",
        "maskedCard": "411111******1111"
      },
      "merchant": {
        "merchantRedirectURL": "https://brethren-kilobyte-deflected.ngrok-free.dev/payment/callback"
      },
      "amount": 250,
      "currency": "EGP"
    },
    "metaData": {
      "kashier payment UI version": "V3",
      "referral url": "https://localhost:7155/",
      "termsAndConditions": {
        "ip": "197.54.250.240"
      }
    },
    "sourceOfFunds": {
      "cardInfo": {
        "maskedCard": "411111******1111",
        "extendedMaskedCard": "41111111****1111",
        "cardBrand": "Visa",
        "cardHolderName": "fathi",
        "cardDataToken": "ee3a569a-d8c9-4fe9-ad34-6b041d8df585",
        "ccvToken": "b14b1e2c-cbb7-4f02-aa4d-058b896f96fd",
        "expiryYear": "27",
        "expiryMonth": "04",
        "storedOnFile": "NOT_STORED",
        "save": false,
        "agreement": null
      }
    },
    "transactionResponseCode": "00",
    "transactionResponseMessage": {
      "en": "Approved",
      "ar": "تمت الموافقة"
    },
    "channel": "online | e-commerce",
    "merchantDetails": {
      "businessEmail": "ahmedmhmd0237@gmail.com"
    },
    "installmentPlan": {},
    "signatureKeys": [
      "amount",
      "channel",
      "creationDate",
      "currency",
      "kashierOrderId",
      "merchantOrderId",
      "method",
      "orderReference",
      "status",
      "transactionId",
      "transactionResponseCode"
    ]
  },
  "hash": "b170247dd7a8df365f41d798f780e18d449e1c37ecb1cbb13e31fbad7bb95249"
}
*/