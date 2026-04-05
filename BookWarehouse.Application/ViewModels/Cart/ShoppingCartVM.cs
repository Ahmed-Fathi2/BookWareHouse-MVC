using BookWarehouse.Application.ViewModels.Product;

namespace BookWarehouse.Application.ViewModels.Cart
{
    public class ShoppingCartVM
    {
        public IEnumerable<ProductReadDetailsVM> CartList { get; set; } = new List<ProductReadDetailsVM>();
        public decimal OrderTotal { get; set; } // Total price of all items in the cart
        public int TotalItems { get; set; } // Total number of items in the cart
    }
}
