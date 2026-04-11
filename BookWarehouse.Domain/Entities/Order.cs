using BookWarehouse.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookWarehouse.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        //Order Details
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus OrderStatus { get; set; }= OrderStatus.Pending;
        public double OrderTotal { get; set; }

        //Shipping Details
        public DateTime ShippingDate { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public string FullName { get; set; }= string.Empty;
        public string PhoneNumber { get; set; }=string.Empty;
        public string StreetAddress { get; set; }=string.Empty;
        public string City { get; set; }= string.Empty;

        //Payment Details
        public PaymentStatus PaymentStatus { get; set; }= PaymentStatus.Pending;
        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = default!;


        public ICollection<OrderDetails> OrderDetails { get; set; } = new HashSet<OrderDetails>();




    }
}
