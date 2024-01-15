using System;
using System.Collections.Generic;
using System.Data;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;
using System.Linq;

namespace Web.Domain.Repositories
{
  public class NonceRepository : INonceRepository
  {
    private IDbTransaction _dbTransaction;

    public NonceRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long AddNonce(Nonce nonce)
    {
      return _dbTransaction.Connection.Insert<Nonce>(nonce, _dbTransaction);
    }

    public string GenerateNonce()
    {
      return Guid.NewGuid().ToString("N");
    }

    public Nonce GetByNonce(string nonce)
    {
      return _dbTransaction.Connection.GetAll<Nonce>(_dbTransaction).Where(x => x.NonceKey == nonce).FirstOrDefault();
    }

    public IEnumerable<Nonce> GetNonce()
    {
      return _dbTransaction.Connection.GetAll<Nonce>(_dbTransaction);
    }

    public bool UpdateNonce(Nonce nonce)
    {
      return _dbTransaction.Connection.Update<Nonce>(nonce, _dbTransaction);
    }
  }
}