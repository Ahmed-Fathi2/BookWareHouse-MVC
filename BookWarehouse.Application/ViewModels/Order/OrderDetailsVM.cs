using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Order
{
    public class OrderDetailsVM
    {

        // 🔹 Order Info
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal OrderTotal { get; set; }

        // 🔹 Payment
        public string? PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        // 🔹 Shipping
        public DateTime? ShippingDate { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        // 🔹 Customer
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string? Email { get; set; } 

        // 🔹 Products
        public List<OrderItemVM> Items { get; set; } = new List<OrderItemVM>();

    }
}
