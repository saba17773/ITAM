using System.Collections.Generic;
using Web.Infrastructure.Entities;

namespace Web.Domain.Interfaces
{
  public interface IEmployeeRepository
  {
    IEnumerable<Employee> GetEmployeeAll();
    Employee GetEmployee(string employeeId);
  }
}