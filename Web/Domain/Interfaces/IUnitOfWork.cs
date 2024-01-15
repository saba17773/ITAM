using System;

namespace Web.Domain.Interfaces
{
  public interface IUnitOfWork : IDisposable
  {
    IUserRepository User { get; }
    IRoleRepository Role { get; }
    IStatusRepository Status { get; }
    IEmployeeRepository Employee { get; }
    ICapabilityRepository Capability { get; }
    IPermissionRepository Permission { get; }
    IApproveRepository Approve { get; }
    ISetupRepository Setup { get; }
    INumberSeqRepository NumberSeq { get; }
    IItemRepository Item { get; }
    INonceRepository Nonce { get; }
    IPartMasterRepository PartMaster { get; }
    void Complete();
  }
}