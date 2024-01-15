using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("NumberSeq")]
  public class NumberSeq
  {
    public int Id { get; set; }

    [Required]
    public string Prefix { get; set; }

    [Required]
    public int Value { get; set; }
  }
}