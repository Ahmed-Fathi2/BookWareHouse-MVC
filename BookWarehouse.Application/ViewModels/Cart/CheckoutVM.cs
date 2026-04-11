using System.ComponentModel.DataAnnotations;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookWarehouse.Application.ViewModels.Cart
{
    public class CheckoutVM
    {
        public decimal OrderTotal { get; set; }



        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string FullName { get; set; }



        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$", ErrorMessage = "Enter a valid Egyptian phone number")]
        public string PhoneNumber { get; set; }




        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address is too long")]
        public string StreetAddress { get; set; } 



        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City name is too long")]
     
        public string City { get; set; } 
    }


}

