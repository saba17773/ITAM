using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class ResetPasswordModel
  {
    [Required]
    public int UserId { get; set; }

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; }

    [Required]
    [MinLength(8)]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; }
  }
}