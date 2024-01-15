using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("PartMaster")]
  public class PartMaster
  {
    public int Id { get; set; }

    [Required]
    public string PartName { get; set; }
    public int Active { get; set; }
  }
}