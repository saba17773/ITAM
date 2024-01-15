using System.Collections.Generic;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface IRoleRepository
  {
    IEnumerable<Role> GetRoleAll();
    IEnumerable<RoleGridModel> GetRoleAllGrid();
    long AddRole(Role role);
    int UpdateRoleGrid(UpdateGridModel updateGrid);
  }
}