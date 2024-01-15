using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;

namespace Web.Domain.Repositories
{
  public class EmployeeRepository : IEmployeeRepository
  {
    private IDbTransaction _dbTransaction;

    public EmployeeRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public Employee GetEmployee(string employeeId)
    {
      return _dbTransaction.Connection
        .GetAll<Employee>(_dbTransaction)
        .Where(x => x.EmployeeId == employeeId)
        .FirstOrDefault();
    }

    public IEnumerable<Employee> GetEmployeeAll()
    {
      return _dbTransaction.Connection.GetAll<Employee>(_dbTransaction);
    }
  }
}