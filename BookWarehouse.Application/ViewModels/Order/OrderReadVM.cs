using BookWarehouse.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Order
{
    public class OrderReadVM
    {

        public int Id { get; set; }
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
      
        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }

        

    }
}
