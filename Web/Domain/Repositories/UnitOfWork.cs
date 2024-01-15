using System;
using System.Data;
using Web.Domain.Interfaces;

namespace Web.Domain.Repositories
{
  public class UnitOfWork : IUnitOfWork
  {
    private IDbConnection _dbConnection;
    private IDbTransaction _dbTransaction;

    public UnitOfWork(IDbConnection dbConnection)
    {
      _dbConnection = dbConnection;
      _dbConnection.Open();
      _dbTransaction = _dbConnection.BeginTransaction();

      User = new UserRepository(_dbTransaction);
      Role = new RoleRepository(_dbTransaction);
      Status = new StatusRepository(_dbTransaction);
      Employee = new EmployeeRepository(_dbTransaction);
      Capability = new CapabilityRepository(_dbTransaction);
      Permission = new PermissionRepository(_dbTransaction);
      Approve = new ApproveRepository(_dbTransaction);
      Setup = new SetupRepository(_dbTransaction);
      NumberSeq = new NumberSeqRepository(_dbTransaction);
      Item = new ItemRepository(_dbTransaction);
      Nonce = new NonceRepository(_dbTransaction);
      PartMaster = new PartMasterRepository(_dbTransaction);
    }

    public IUserRepository User { get; private set; }
    public IRoleRepository Role { get; private set; }
    public IStatusRepository Status { get; private set; }
    public IEmployeeRepository Employee { get; private set; }
    public ICapabilityRepository Capability { get; private set; }
    public IPermissionRepository Permission { get; private set; }
    public IApproveRepository Approve { get; private set; }
    public ISetupRepository Setup { get; private set; }
    public INumberSeqRepository NumberSeq { get; private set; }
    public IItemRepository Item { get; private set; }
    public INonceRepository Nonce { get; private set; }
    public IPartMasterRepository PartMaster { get; private set; }

    public void Complete()
    {
      try
      {
        _dbTransaction.Commit();
      }
      catch
      {
        _dbTransaction.Rollback();
        throw;
      }
      finally
      {
        _dbTransaction.Dispose();
      }
    }

    public void Dispose()
    {
      if (_dbTransaction != null)
      {
        _dbTransaction.Dispose();
        _dbTransaction = null;
      }

      if (_dbConnection != null)
      {
        _dbConnection.Dispose();
        _dbConnection = null;
      }
    }
  }
}