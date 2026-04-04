using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Cart
{
    public class CreateCartVM
    {

        public Guid ProductId { get; set; }

        //[Range(1, 100, ErrorMessage = "Count must be at least 1 and Max 100")]
        public int Count { get; set; }
    }
}
