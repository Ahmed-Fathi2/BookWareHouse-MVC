using System.ComponentModel.DataAnnotations;

namespace BookWarehouse.Application.ViewModels.Role
{
    public class CreateRoleVM
    {
        [Required(ErrorMessage = "Role name is required")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; } = string.Empty;
    }
}
