using BookWarehouse.Application.Abstractions;
using BookWarehouse.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BookWarehouse.Presentation.Controllers
{
    [Route("stripe/webhook")]
    [ApiController]
    public class StripeWebhookController(IStripePaymentService stripePaymentService) : Controller
    {

        private readonly IStripePaymentService _stripePaymentService = stripePaymentService;

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            await _stripePaymentService.HandleStripeWebhookAsync(json, signature!);

            return Ok();
        }
    }
}
/*
 Stripe Webhook Implementation for Production
How Webhooks Work
Instead of trusting the redirect from the browser (which can be faked), Stripe's server calls your server directly when payment succeeds — no user involved.
User pays on Stripe
       ↓
Stripe server → POST /webhook → Your server (backend only)
       ↓
Your server confirms payment and updates DB

Step 1 — Create the Webhook Endpoint
csharp[Route("stripe/webhook")]
[ApiController]
public class StripeWebhookController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(
        IUnitOfWork unitOfWork,
        IConfiguration config,
        ILogger<StripeWebhookController> logger)
    {
        _unitOfWork = unitOfWork;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        // 1. Read raw body
        var json = await new StreamReader(HttpContext.Request.Body)
            .ReadToEndAsync();

        // 2. Get Stripe signature from header
        var stripeSignature = Request.Headers["Stripe-Signature"];

        // 3. Get your webhook secret from config
        var webhookSecret = _config["Stripe:WebhookSecret"];

        Event stripeEvent;

        try
        {
            // 4. Verify the event came from Stripe (not a fake request)
            stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                webhookSecret
            );
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook signature verification failed");
            return BadRequest("Invalid signature");
        }

        // 5. Handle the event type
        switch (stripeEvent.Type)
        {
            case Events.CheckoutSessionCompleted:
                await HandleCheckoutSessionCompleted(stripeEvent);
                break;

            case Events.PaymentIntentPaymentFailed:
                await HandlePaymentFailed(stripeEvent);
                break;

            default:
                _logger.LogInformation("Unhandled Stripe event: {Type}", stripeEvent.Type);
                break;
        }

        // 6. Always return 200 to Stripe — otherwise it retries
        return Ok();
    }

    private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
    {
        var session = stripeEvent.Data.Object as Session;

        if (session == null) return;

        // Find the order by SessionId
        var orderHeader = _unitOfWork.OrderHeader
            .Get(o => o.SessionId == session.Id, includeProperties: "ApplicationUser");

        if (orderHeader == null)
        {
            _logger.LogWarning("Order not found for session {SessionId}", session.Id);
            return;
        }

        // Avoid processing the same event twice (idempotency)
        if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        {
            _logger.LogInformation("Order {Id} already approved, skipping", orderHeader.Id);
            return;
        }

        if (session.PaymentStatus == "paid")
        {
            _unitOfWork.OrderHeader.UpdateStripePaymentID(
                orderHeader.Id, session.Id, session.PaymentIntentId);

            _unitOfWork.OrderHeader.UpdateStatus(
                orderHeader.Id, SD.StatusApproved, SD.PaymentStatusApproved);

            _unitOfWork.Save();

            _logger.LogInformation("Order {Id} approved via webhook", orderHeader.Id);
        }
    }

    private async Task HandlePaymentFailed(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        if (paymentIntent == null) return;

        var orderHeader = _unitOfWork.OrderHeader
            .Get(o => o.PaymentIntentId == paymentIntent.Id);

        if (orderHeader != null)
        {
            _unitOfWork.OrderHeader.UpdateStatus(
                orderHeader.Id, SD.StatusCancelled, SD.PaymentStatusRejected);

            _unitOfWork.Save();

            _logger.LogWarning("Payment failed for Order {Id}", orderHeader.Id);
        }
    }
}

Step 2 — Critical: Disable Body Buffering
Stripe signature verification needs the raw request body. ASP.NET reads and disposes it by default — you must tell it not to.
csharp// Add this attribute to your webhook action
[HttpPost]
[DisableRequestSizeLimit]
public async Task<IActionResult> Index()
{
    // ...
}
And in Program.cs, add this before app.UseRouting():
csharp// Allow Stripe webhook to read raw body
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/stripe/webhook"))
    {
        context.Request.EnableBuffering();
    }
    await next();
});

Step 3 — Exclude Webhook from CSRF & Auth
csharp// Program.cs — exclude webhook from antiforgery
builder.Services.AddControllersWithViews(options =>
{
    // already handled by [ApiController]
});

// The webhook route should NOT require authorization
// Put this BEFORE app.UseAuthorization()
app.MapControllerRoute("webhook", "stripe/webhook")
   .AllowAnonymous();
Or simply put [AllowAnonymous] on the controller — Stripe authenticates via signature, not login.

Step 4 — appsettings.json
json{
  "Stripe": {
    "PublishableKey": "pk_live_...",
    "SecretKey": "sk_live_...",
    "WebhookSecret": "whsec_..."   ← get this from Stripe Dashboard
  }
}

Step 5 — Update OrderConfirmation (Remove Payment Logic)
Now that the webhook handles payment confirmation, your OrderConfirmation action becomes simpler and just shows the result:
csharppublic IActionResult OrderConfirmation(int id)
{
    // Just display the page — webhook already handled payment
    // But add a small safety check for edge cases
    OrderHeader orderHeader = _unitOfWork.OrderHeader
        .Get(u => u.Id == id, includeProperties: "ApplicationUser");

    if (orderHeader == null)
        return NotFound();

    // Clear cart only if not already cleared
    // (webhook may have already done this — be idempotent)
    if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
    {
        var shoppingCarts = _unitOfWork.ShoppingCart
            .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId)
            .ToList();

        if (shoppingCarts.Any())
        {
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
        }

        HttpContext.Session.Clear();
    }

    return View(id);
}

Step 6 — Get the Webhook Secret from Stripe Dashboard
Stripe Dashboard
  → Developers
    → Webhooks
      → Add Endpoint
        URL: https://yourdomain.com/stripe/webhook
        Events to listen to:
          ✅ checkout.session.completed
          ✅ payment_intent.payment_failed
      → After creating → copy "Signing secret" (whsec_...)
        → paste in appsettings.json

Step 7 — Test Locally with Stripe CLI
bash# Install Stripe CLI
# Then run:
stripe listen --forward-to https://localhost:7001/stripe/webhook

# In another terminal, trigger a test event:
stripe trigger checkout.session.completed
The CLI gives you a temporary webhook secret for local testing — it prints it when you run stripe listen.

The Full Production Flow
1. User clicks Pay
2. You create Stripe Session → redirect to Stripe (303)
3. User pays on Stripe
4. Stripe calls POST /stripe/webhook (server to server, no user involved)
5. You verify signature → update DB → approve order
6. Stripe redirects user to SuccessUrl (OrderConfirmation)
7. OrderConfirmation just shows the result (DB already updated by webhook)

Key Rules to Remember
RuleWhyAlways verify Stripe-SignatureRejects fake requestsAlways return 200 OK to StripeOtherwise Stripe retries for 3 daysCheck if already processed (idempotency)Stripe can send the same event more than onceUse raw body for verificationParsed body breaks the signature checkStore WebhookSecret in environment variablesNever in source code
 
 */