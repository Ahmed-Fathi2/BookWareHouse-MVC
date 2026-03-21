using System.ComponentModel.DataAnnotations; 

namespace BookWarehouse.Application.ViewModels.Product
{
    public class ProductReadDetailsVM
    {
        public Guid Id { get; set; }

        [Display(Name = "Product Title")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Display(Name = "ISBN Code")]
        public string ISBN { get; set; } = string.Empty;


        public string Author { get; set; } = string.Empty;

        [Display(Name = "List Price")]
        public decimal ListPrice { get; set; } // Price Before Discount



        [Display(Name = "Price for 1-50 units")]
        public decimal Price { get; set; }



        [Display(Name = "Price for 51-100 units")]
        public decimal Price50 { get; set; }



        [Display(Name = "Price for 100+ units")]
        public decimal Price100 { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = string.Empty;
    }
}