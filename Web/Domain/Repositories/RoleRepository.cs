using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Repositories
{
  public class RoleRepository : IRoleRepository
  {
    private IDbTransaction _dbTransaction;

    public RoleRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long AddRole(Role role)
    {
      return _dbTransaction.Connection.Insert<Role>(role, _dbTransaction);
    }

    public IEnumerable<Role> GetRoleAll()
    {
      return _dbTransaction.Connection.GetAll<Role>(_dbTransaction);
    }

    public IEnumerable<RoleGridModel> GetRoleAllGrid()
    {
      return _dbTransaction.Connection.Query<RoleGridModel>(@"
        SELECT 
          R.Id,
          R.[Description],
          S.[Description] AS Active
        FROM DSG_Role R
        LEFT JOIN DSG_Status S 
          ON S.Id = R.Active
      ", null, _dbTransaction);
    }

    public int UpdateRoleGrid(UpdateGridModel updateGrid)
    {
      return _dbTransaction.Connection.Execute(@"
        UPDATE DSG_Role
        SET " + updateGrid.name + @" = @Value
        WHERE Id = @Id
      ", new
      {
        @Value = updateGrid.value,
        @Id = updateGrid.pk
      }, _dbTransaction);
    }
  }
}