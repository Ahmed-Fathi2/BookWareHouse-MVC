using BookWarehouse.Application.Abstractions;
using BookWarehouse.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BookWarehouse.Presentation.Controllers
{
    [Route("stripe/webhook")]
    [ApiController]
    public class StripeWebhookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly IStripePaymentService _stripePaymentService;

        public StripeWebhookController(
            IUnitOfWork unitOfWork,
            IConfiguration config,
            ILogger<StripeWebhookController> logger,
            IStripePaymentService stripePaymentService)

        {
            _unitOfWork = unitOfWork;
            _config = config;
            _logger = logger;
            _stripePaymentService = stripePaymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            //await _stripePaymentService.HandleStripeWebhookAsync(json, signature!);

            return Ok();
        }
    }
}
