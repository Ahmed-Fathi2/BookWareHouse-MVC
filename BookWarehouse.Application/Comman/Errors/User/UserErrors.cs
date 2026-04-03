using Error = BookWarehouse.Application.Comman.Results.Error;

namespace BookWarehouse.Application.Comman.Errors.User
{
    public static class UserErrors
    {

        public static readonly Error EmailAlreadyExists = new("UserEmailAlreadyExists", "Email already exists");
        public static readonly Error UserCreationFailed = new("UserCreationFailed", "Failed to create user");

        public static readonly Error InvalidCredentials = new("InvalidEmailOrPassword", "Invalid email or password");
        public static readonly Error EmailNotConfirmed = new("EmailNotConfirmed", "Email not confirmed");
        public static readonly Error UserLockedOut = new("UserLockedOut", "Your account has been temporarily locked. Please try again later");

        public static readonly Error UserNotFound = new("UserNotFound", "User not found");
        public static readonly Error InvalidCode = new("InvalidCode", "Invalid code");

        
    }
}
