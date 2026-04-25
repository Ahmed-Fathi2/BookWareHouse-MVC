using BookWarehouse.Application.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Product
{
    public class ProductQueryVM
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 9;


        public string? SearchValue { get; set; }  // filter by name or description

        public int? CategoryId { get; set; } // filter by category


        public decimal? MinPrice { get; set; } // filter by minimum price

        public decimal? MaxPrice { get; set; } // filter by maximum price


        public string? SortBy { get; set; }  // sort by property name
        public bool IsDescending { get; set; } = false;




    }
}
