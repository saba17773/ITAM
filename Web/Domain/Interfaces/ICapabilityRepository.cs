using System.Collections.Generic;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface ICapabilityRepository
  {
    IEnumerable<Capability> GetCapabilityAll();
    int UpdateCapabilityGrid(UpdateGridModel updateGrid);
    long AddCapability(Capability capability);
  }
}