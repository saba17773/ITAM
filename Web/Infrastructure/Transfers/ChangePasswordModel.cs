using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class ChangePasswordModel
  {
    [Required]
    public string OldPassword { get; set; }

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; }

    [Required]
    [MinLength(8)]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; }
  }
}