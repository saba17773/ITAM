using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("DSG_Status")]
  public class Status
  {
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }
  }
}