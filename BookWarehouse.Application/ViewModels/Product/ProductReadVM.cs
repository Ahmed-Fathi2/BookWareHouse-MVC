using System.ComponentModel.DataAnnotations;

namespace BookWarehouse.Application.ViewModels.Product
{
    public class ProductReadVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


        public string ISBN { get; set; } = string.Empty;


        [Display(Name = "List Price")]
        public decimal Price { get; set; } // Final price after discount

        public decimal ListPrice { get; set; }

        public string Author { get; set; } = string.Empty;


        public  int  CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

    }
}
