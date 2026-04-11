using System.ComponentModel.DataAnnotations;

namespace BookWarehouse.Application.ViewModels.Cart
{
    public class CartDetailsVM
    {
        public int Id { get; set; } // Cart Id


        public int ProductId { get; set; }

        [Display(Name = "Product Title")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


        public string Author { get; set; } = string.Empty;

        [Display(Name = "List Price")]
        public decimal ListPrice { get; set; } // Price Before Discount



        [Display(Name = "Price for 1-50 units")]
        public decimal Price { get; set; }



        [Display(Name = "Price for 51-100 units")]
        public decimal Price50 { get; set; }



        [Display(Name = "Price for 100+ units")]
        public decimal Price100 { get; set; }


       
        public decimal FinalPrice { get; set; } // Final price based on quantity

        public string ImageUrl { get; set; } = string.Empty;

        public int Count { get; set; }

  
    }
}
