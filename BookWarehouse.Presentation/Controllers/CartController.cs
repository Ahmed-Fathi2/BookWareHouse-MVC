using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookWarehouse.Presentation.Controllers
{
    public class CartController(ICartService cartService) : Controller
    {
        private readonly ICartService _cartService = cartService;


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartProductsResult = await _cartService.GetAllUserCartProducts(userId!);

            var shoppingCartVM = new ShoppingCartVM()
            {
                CartList = cartProductsResult.Value,
                OrderTotal = cartProductsResult.Value.Sum(p => GetPriceBasedOnQuantity(p)),
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

        public async Task<IActionResult> DeleteFromCart(Guid Id)
        {
            var result = await _cartService.DeleteFromCart(Id);

            return result.IsSuccess ? RedirectToAction("Index") : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Plus(Guid Id)
        {
            var result = await _cartService.IncreaseQuantity(Id);
            if (!result.IsSuccess) return Json(new { success = false });

            return await GetCartUpdateJsonResult(Id);
        }

        [HttpPost]
        public async Task<IActionResult> Minus(Guid Id)
        {
            var result = await _cartService.DecreaseQuantity(Id);
            if (!result.IsSuccess) return Json(new { success = false });

            return await GetCartUpdateJsonResult(Id);
        }


        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.GetAllUserCartProducts(userId!);
            
            var checkoutVM = new CheckoutVM
            {
                OrderTotal = cart.Value.Sum(p => GetPriceBasedOnQuantity(p))
            };
            
            return View(checkoutVM);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutVM checkoutVM)
        {
            if (!ModelState.IsValid)
            {
                return View(checkoutVM);
            }
            
            // TODO: Process Order logic here
            return RedirectToAction("Index", "Home");
        }

        private async Task<IActionResult> GetCartUpdateJsonResult(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _cartService.GetAllUserCartProducts(userId!);

            var newTotal = cart.Value.Sum(p => GetPriceBasedOnQuantity(p));

            var item = cart.Value.FirstOrDefault(p => p.Id == id);

            var newQty = item?.Count ?? 0;

            var newItemPrice = item != null ? GetPriceBasedOnQuantity(item) : 0;

            return Json(new { success = true, newTotal, newQty, newItemPrice });
        }

        private decimal GetPriceBasedOnQuantity(CartDetailsVM cartProduct)
        {

            if (cartProduct.Count <= 50)
            {
                return cartProduct.Price * cartProduct.Count;
            }
            else if (cartProduct.Count <= 100)
            {
                return cartProduct.Price50 * cartProduct.Count;
            }
            else
            {
                return cartProduct.Price100 * cartProduct.Count;
            }

        }
    }
}
