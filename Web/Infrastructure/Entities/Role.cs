using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("DSG_Role")]
  public class Role
  {
    public int Id { get; set; }
    public string Description { get; set; }

    [Required]
    public int Active { get; set; }
  }
}