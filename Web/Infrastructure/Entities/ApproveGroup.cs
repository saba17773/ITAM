using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("ApproveGroup")]
  public class ApproveGroup
  {
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }
    public string Slug { get; set; }
  }
}