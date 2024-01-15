using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("DSG_User")]
  public class User
  {
    public int Id { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(10)]
    public string Username { get; set; }

    [Required]
    [MinLength(8)]
    [MaxLength(20)]
    public string Password { get; set; }

    [Required]
    [MaxLength(20)]
    public string EmployeeId { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public int Active { get; set; }

    [Required]
    public int RoleId { get; set; }

    [MaxLength(100)]
    public string Remark { get; set; }
  }
}