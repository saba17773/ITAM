using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class AddRoleModel
  {
    [Required]
    public string RoleDescription { get; set; }
  }
}