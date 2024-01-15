using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("DSG_Permission")]
  public class Permission
  {
    public int Id { get; set; }

    [Required]
    public int RoleId { get; set; }

    [Required]
    public int CapabilityId { get; set; }
  }
}