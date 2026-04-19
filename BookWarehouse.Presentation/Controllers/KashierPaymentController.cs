using BookWarehouse.Application.Abstractions;
using BookWarehouse.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BookWarehouse.Presentation.Controllers
{
    public class KashierPaymentController(IPaymentService paymentService , IOrderService orderService) : Controller
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IOrderService _orderService = orderService;



        [HttpGet("payment/callback")]
        public IActionResult Callback()
        {
            var orderId = Request.Query["merchantOrderId"];
            var status = Request.Query["paymentStatus"];

            Console.WriteLine($"OrderId: {orderId}");
            Console.WriteLine($"Status: {status}");

            return RedirectToAction("Index", "Home"); 
        }


        [HttpPost("api/payment/webhook")]
        public async Task<IActionResult> Webhook()
        {
            var body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(); // read raw body as string

            var receivedSignature = Request.Headers["x-kashier-signature"].ToString();

            if (string.IsNullOrEmpty(receivedSignature))
                return Ok();

            var result = await _paymentService.HandleWebhookAsync(body, receivedSignature);

            if (!result.IsSuccess)
                return Ok();

            await _orderService.HandlePaymentResult(result.Value);
            return Ok();



        }

    }

}
// Request Body
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