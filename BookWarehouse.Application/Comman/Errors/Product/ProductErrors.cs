using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Comman.Errors.Product
{
    public class ProductErrors
    {
        public static readonly Error NotFound = new Error("Product.NotFound", "The specified product was not found.");
    }
}
