using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookWarehouse.Presentation.Controllers
{
    public class CartController(ICartService cartService,IOrderService orderService) : Controller
    {
        private readonly ICartService _cartService = cartService;
        private readonly IOrderService _orderService = orderService;


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartProductsResult = await _cartService.GetAllUserCartProducts(userId!);

            var shoppingCartVM = new ShoppingCartVM()
            {
                CartList = cartProductsResult.Value,
                //OrderTotal = cartProductsResult.Value.Sum(p => GetPriceBasedOnQuantity(p)),
                OrderTotal = cartProductsResult.Value.Sum(p =>p.FinalPrice),
                TotalItems = cartProductsResult.Value.Count()
            };

            return View(shoppingCartVM);

        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(CreateCartVM createCartVM)
        {

            createCartVM.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _cartService.AddToCart(createCartVM);

            TempData["success"] = "Added to cart successfully";

            return RedirectToAction("Index", "CustomerProduct");
        }


        [HttpPost]
        public async Task<IActionResult> Plus(int Id)
        {
            var result = await _cartService.IncreaseQuantity(Id);
            if (!result.IsSuccess) return Json(new { success = false });

            return await GetCartUpdateJsonResult(Id);
        }

        [HttpPost]
        public async Task<IActionResult> Minus(int Id)
        {
            var result = await _cartService.DecreaseQuantity(Id);
            if (!result.IsSuccess) return Json(new { success = false });

            return await GetCartUpdateJsonResult(Id);
        }

        public async Task<IActionResult> DeleteFromCart(int Id)
        {
            var result = await _cartService.DeleteFromCart(Id);

            return result.IsSuccess ? RedirectToAction("Index") : NotFound();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.GetAllUserCartProducts(userId!);
            
            var checkoutVM = new CheckoutVM
            {
                OrderTotal = cart.Value.Sum(p => p.FinalPrice)
            };
            
            return View(checkoutVM);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutVM checkoutVM)
        {
            // Dont Forget To Delete Cart After Placing Order

            if (!ModelState.IsValid)
                return View(checkoutVM);
            

            checkoutVM.ApplicationUserId= User.FindFirstValue(ClaimTypes.NameIdentifier)!;

             var origin = $"{Request.Scheme}://{Request.Host}";

            var sessionUrlResult = await _orderService.PlaceOrderAsync(origin,checkoutVM);

          
            Response.Headers.Append("Location", sessionUrlResult.Value);

            return new StatusCodeResult(303);

            //return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int id)
        {
            return Content($"Order with id {id} has been placed successfully!");
        }

        private async Task<IActionResult> GetCartUpdateJsonResult(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _cartService.GetAllUserCartProducts(userId!);

            var newTotal = cart.Value.Sum(p => p.FinalPrice);

            var item = cart.Value.FirstOrDefault(p => p.Id == id);

            var newQty = item?.Count ?? 0;

            var newItemPrice = item != null ? item.FinalPrice : 0;

            return Json(new { success = true, newTotal, newQty, newItemPrice });
        }

        //private decimal GetPriceBasedOnQuantity(CartDetailsVM cartProduct)
        //{

        //    if (cartProduct.Count <= 50)
        //    {
        //        return cartProduct.Price * cartProduct.Count;
        //    }
        //    else if (cartProduct.Count <= 100)
        //    {
        //        return cartProduct.Price50 * cartProduct.Count;
        //    }
        //    else
        //    {
        //        return cartProduct.Price100 * cartProduct.Count;
        //    }

        //}
    }
}
