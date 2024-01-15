using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Repositories
{
  public class CapabilityRepository : ICapabilityRepository
  {
    private IDbTransaction _dbTransaction;

    public CapabilityRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long AddCapability(Capability capability)
    {
      return _dbTransaction.Connection.Insert<Capability>(capability, _dbTransaction);
    }

    public IEnumerable<Capability> GetCapabilityAll()
    {
      return _dbTransaction.Connection.GetAll<Capability>(_dbTransaction);
    }

    public int UpdateCapabilityGrid(UpdateGridModel updateGrid)
    {
      return _dbTransaction.Connection.Execute(@"
        UPDATE DSG_Capability
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