using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class UserLoginModel
  {
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
    public int EmployeeId { get; set; }
    public string Type { get; set; }
  }
}