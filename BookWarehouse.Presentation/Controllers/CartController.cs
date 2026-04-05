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

   


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(CreateCartVM createCartVM)
        {

            createCartVM.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _cartService.AddToCart(createCartVM);

            TempData["success"] = "Added to cart successfully";

            return RedirectToAction("Index", "CustomerProduct");
        }
    }
}
