using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyPortfolio.Models.ManagerModels.RoleModels
{
    public class CreateEditRoleViewModel
    {
        [Required(ErrorMessage = "Role name field Is Required")]
        [StringLength(256, ErrorMessage = "Role name must be 256 characters or fewer.")]
        public string RoleName { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> AvailableRoles { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
