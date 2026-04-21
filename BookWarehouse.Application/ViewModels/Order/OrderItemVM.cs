namespace BookWarehouse.Application.ViewModels.Order
{
    public class OrderItemVM
    {
        public string Title { get; set; }        // Product Name
        public string ImageUrl { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }       // Unit Price

        public decimal OrderTotal => Quantity * Price;
    }
}
