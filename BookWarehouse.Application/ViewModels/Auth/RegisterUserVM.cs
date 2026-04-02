using System.ComponentModel.DataAnnotations;

namespace Ecom.BLL.ViewModel.Authentication
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        [MaxLength(20, ErrorMessage = "First name must not exceed 20 characters.")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
        [MaxLength(20, ErrorMessage = "Last name must not exceed 20 characters.")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Username is required.")]
        //[MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        //[MaxLength(20, ErrorMessage = "Username must not exceed 20 characters.")]
        //public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }


    }
}