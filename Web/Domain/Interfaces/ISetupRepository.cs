using System.Collections.Generic;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface ISetupRepository
  {
    IEnumerable<ApproveSetupGridModel> GetApproveSetupGrid();
    IEnumerable<ApproveSetup> GetApproveSetup();
    int UpdateApproveSetupGrid(UpdateGridModel updateGrid);
    bool UpdateApproveSetup(ApproveSetup approveSetup);
    long AddApproveSetup(ApproveSetup approveSetup);
    IEnumerable<ApproveGroup> GetApproveGroup();
  }
}