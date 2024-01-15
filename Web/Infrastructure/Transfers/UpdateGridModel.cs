using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class UpdateGridModel
  {
    [Required]
    public int pk { get; set; }

    [Required]
    public string name { get; set; }
    public string value { get; set; }
  }
}