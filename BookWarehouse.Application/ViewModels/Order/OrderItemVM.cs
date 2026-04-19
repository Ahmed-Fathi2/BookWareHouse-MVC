namespace BookWarehouse.Application.ViewModels.Order
{
    public class OrderItemVM
    {
        public string Title { get; set; }        // Product Name
        public int Quantity { get; set; }
        public decimal Price { get; set; }       // Unit Price

        public decimal OrderTotal => Quantity * Price;
    }
}
