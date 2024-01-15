using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class FormAddApproveGroupModel
  {
    [Required]
    public int UserId { get; set; }

    [Required]
    public int GroupId { get; set; }
  }
}