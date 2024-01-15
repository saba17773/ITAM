using System.Collections.Generic;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface IPartMasterRepository
  {
    IEnumerable<PartMaster> GetPartMaster();
    PartMaster GetPartMaster(int id);
    long AddPartMaster(PartMaster partMaster);
    bool UpdatePartMaster(PartMaster partMaster);
    int UpdatePartMasterGrid(UpdateGridModel updateGrid);
  }
}