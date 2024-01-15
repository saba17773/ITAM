using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class PermissionModel
  {
    [Required]
    public int Id { get; set; }
    public string Capability { get; set; }

    [Required]
    public string Slug { get; set; }
    public int RoleId { get; set; }
  }
}