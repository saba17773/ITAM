using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class ApproveResultModel
  {
    [Required]
    public int TransId { get; set; }

    [Required]
    public int ApproveResult { get; set; }
    public string Remark { get; set; }
  }
}