using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("DSG_Employee")]
  public class Employee
  {
    public int Id { get; set; }
    public string EmployeeId { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string NameEng { get; set; }
    public string Company { get; set; }
    public string PositionCode { get; set; }
    public string PositionName { get; set; }
    public string DivisionCode { get; set; }
    public string DivisionName { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
    public int Status { get; set; }
    public string EmployeeIdOld { get; set; }
    public string Email { get; set; }
  }
}