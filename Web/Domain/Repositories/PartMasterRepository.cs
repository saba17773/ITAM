using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Repositories
{
  public class PartMasterRepository : IPartMasterRepository
  {
    private IDbTransaction _dbTransaction;

    public PartMasterRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long AddPartMaster(PartMaster partMaster)
    {
      return _dbTransaction.Connection.Insert<PartMaster>(partMaster, _dbTransaction);
    }

    public IEnumerable<PartMaster> GetPartMaster()
    {
      return _dbTransaction.Connection.GetAll<PartMaster>(_dbTransaction);
    }

    public PartMaster GetPartMaster(int id)
    {
      return _dbTransaction.Connection.Get<PartMaster>(id, _dbTransaction);
    }

    public bool UpdatePartMaster(PartMaster partMaster)
    {
      return _dbTransaction.Connection.Update<PartMaster>(partMaster, _dbTransaction);
    }

    public int UpdatePartMasterGrid(UpdateGridModel updateGrid)
    {
      return _dbTransaction.Connection.Execute(@"
        UPDATE PartMaster
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