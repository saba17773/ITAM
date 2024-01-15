using System.Collections.Generic;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface IPermissionRepository
  {
    long AddPermission(Permission permission);
    IEnumerable<PermissionModel> GetPermissionByRole(int roleId);
    bool DeletePermission(Permission permission);
    IEnumerable<Permission> GetPermissionAll();
  }
}