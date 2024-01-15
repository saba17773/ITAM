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
  public class PermissionRepository : IPermissionRepository
  {
    private IDbTransaction _dbTransaction;

    public PermissionRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long AddPermission(Permission permission)
    {
      return _dbTransaction.Connection.Insert<Permission>(permission, _dbTransaction);
    }

    public bool DeletePermission(Permission permission)
    {
      return _dbTransaction.Connection.Delete<Permission>(permission, _dbTransaction);
    }

    public IEnumerable<Permission> GetPermissionAll()
    {
      return _dbTransaction.Connection.GetAll<Permission>(_dbTransaction);
    }

    public IEnumerable<PermissionModel> GetPermissionByRole(int roleId)
    {
      return _dbTransaction.Connection.Query<PermissionModel>(@"
        SELECT 
          C.Id,
          C.[Description] AS Capability,
          C.Slug,
          CASE 
          WHEN P.RoleId IS NULL THEN NULL
          ELSE P.RoleId
          END AS RoleId
        FROM DSG_Capability C
        LEFT JOIN DSG_Permission P ON P.CapabilityId = C.Id
        AND P.RoleId = @RoleId
        OR P.RoleId IS NULL
      ", new { @RoleId = roleId }, _dbTransaction);
    }
  }
}