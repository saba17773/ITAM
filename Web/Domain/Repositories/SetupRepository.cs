using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Repositories
{
  public class SetupRepository : ISetupRepository
  {
    private IDbTransaction _dbTransaction;

    public SetupRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long AddApproveSetup(ApproveSetup approveSetup)
    {
      return _dbTransaction.Connection.Insert<ApproveSetup>(approveSetup, _dbTransaction);
    }

    public IEnumerable<ApproveGroup> GetApproveGroup()
    {
      return _dbTransaction.Connection.GetAll<ApproveGroup>(_dbTransaction);
    }

    public IEnumerable<ApproveSetup> GetApproveSetup()
    {
      return _dbTransaction.Connection.GetAll<ApproveSetup>(_dbTransaction);
    }

    public IEnumerable<ApproveSetupGridModel> GetApproveSetupGrid()
    {
      string sql = @"SELECT 
        A.Id,
        E.Name + ' ' + E.LastName AS Employee,
        U.Email,
        AG.[Description] AS ApproveGroup,
        A.ApproveLevel,
        A.NotifyTo,
        A.FinalApprove,
        A.CanApprove,
        A.EmailBackup,
        A.Active,
        E.Company
        FROM ApproveSetup A
        LEFT JOIN DSG_User U 
        ON A.UserId = U.Id
        LEFT JOIN DSG_Employee E
        ON E.EmployeeId = U.EmployeeId
        LEFT JOIN ApproveGroup AG
        ON AG.Id = A.ApproveGroup";

      return _dbTransaction.Connection.Query<ApproveSetupGridModel>(sql, null, _dbTransaction);
    }

    public bool UpdateApproveSetup(ApproveSetup approveSetup)
    {
      return _dbTransaction.Connection.Update<ApproveSetup>(approveSetup, _dbTransaction);
    }

    public int UpdateApproveSetupGrid(UpdateGridModel updateGrid)
    {
      return _dbTransaction.Connection.Execute(@"
        UPDATE ApproveSetup
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