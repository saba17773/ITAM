using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;

namespace Web.Domain.Repositories
{
  public class NumberSeqRepository : INumberSeqRepository
  {
    private IDbTransaction _dbTransaction;

    public NumberSeqRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long AddNumberSeq(NumberSeq numberSeq)
    {
      return _dbTransaction.Connection.Insert<NumberSeq>(numberSeq, _dbTransaction);
    }

    public NumberSeq GetNumberSeq(string key)
    {
      return _dbTransaction.Connection
        .GetAll<NumberSeq>(_dbTransaction)
        .Where(x => x.Prefix == key)
        .FirstOrDefault();
    }

    public bool UpdateNumberSeq(NumberSeq numberSeq)
    {
      return _dbTransaction.Connection.Update<NumberSeq>(numberSeq, _dbTransaction);
    }
  }
}