using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Cart
{
    public class CreateCartVM
    {
        public string? ApplicationUserId { get; set; } 
        public Guid ProductId { get; set; }

        public int Count { get; set; }
    }
}
