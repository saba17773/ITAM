using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("DSG_Capability")]
  public class Capability
  {
    public int Id { get; set; }
    public string Description { get; set; }

    [Required]
    public string Slug { get; set; }
  }
}