using BookWarehouse.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    [Route("stripe/webhook")]
    [ApiController]
    public class StripePaymentController(IPaymentService paymentService, IOrderService orderService) : Controller
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IOrderService _orderService = orderService;

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var receivedSignature = Request.Headers["Stripe-Signature"];

            if (string.IsNullOrEmpty(receivedSignature))
                return Ok();

            var result = await _paymentService.HandleWebhookAsync(body, receivedSignature!);

            if (!result.IsSuccess)
                return Ok();

            await _orderService.HandlePaymentResult(result.Value);
            return Ok();
        }
    }
}
