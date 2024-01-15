using System.Collections.Generic;
using System.Data;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;

namespace Web.Domain.Repositories
{
  public class StatusRepository : IStatusRepository
  {
    private IDbTransaction _dbTransaction;

    public StatusRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public IEnumerable<Status> GetStatusAll()
    {
      return _dbTransaction.Connection.GetAll<Status>(_dbTransaction);
    }
  }
}