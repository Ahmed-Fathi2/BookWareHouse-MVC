using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Product
{
    public class ProductEditVM
    {

        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        [MaxLength(1024)]

        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string ISBN { get; set; } = string.Empty;



        [Required]
        [MaxLength(256)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [Display(Name = "List Price")]
        [DataType(DataType.Currency)]
        public decimal ListPrice { get; set; } // Price Before Discount

        [Required]
        [Display(Name = "Price for 1-50 units")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } // Price for 1-50 units

        [Required]
        [Display(Name = "Price for 51-100 units")]
        [DataType(DataType.Currency)]
        public decimal Price50 { get; set; } // Price for 51-100 units

        [Required]
        [Display(Name = "Price for 100+ units")]
        [DataType(DataType.Currency)]
        public decimal Price100 { get; set; } // Price for more than 100  units

        [Required]
        [Display(Name = "Category Name")]
        public Guid CategoryId { get; set; }

        [Display(Name ="Book Image")]
        public string? ImageUrl { get; set; }


    }
}
