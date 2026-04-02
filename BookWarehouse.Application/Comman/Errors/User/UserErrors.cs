using Error = BookWarehouse.Application.Comman.Results.Error;

namespace BookWarehouse.Application.Comman.Errors.User
{
    public static class UserErrors
    {

        public static readonly Error EmailAlreadyExists = new("User.EmailAlreadyExists", "Email already exists");
        public static readonly Error UserCreationFailed = new("UserCreationFailed", "Failed to create user");
    }
}
