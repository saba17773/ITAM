using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class UserClaimModel
  {
    [Required]
    public int UserId { get; set; }

    [Required]
    public int RoleId { get; set; }
    public string Username { get; set; }
    public string EmployeeId { get; set; }
  }
}