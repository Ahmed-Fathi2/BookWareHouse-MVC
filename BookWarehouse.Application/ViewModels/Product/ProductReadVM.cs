using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Product
{
    public class ProductReadVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


        public string ISBN { get; set; } = string.Empty;


        [Display(Name = "List Price")]
        public decimal Price { get; set; }

        public decimal ListPrice { get; set; }

        public string Author { get; set; } = string.Empty;


        public string CategoryName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

    }
}
