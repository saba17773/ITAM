using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Repositories
{
  public class ApproveRepository : IApproveRepository
  {
    private IDbTransaction _dbTransaction;

    public ApproveRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long GenerateApproveTrans(IEnumerable<ApproveTrans> approveTrans)
    {
      return _dbTransaction.Connection.Insert<IEnumerable<ApproveTrans>>(approveTrans, _dbTransaction);
    }

    public IEnumerable<ApproveGroup> GetApproveGroup()
    {
      return _dbTransaction.Connection.GetAll<ApproveGroup>(_dbTransaction);
    }

    public IEnumerable<ApproveListsModel> GetApproveLists(string email)
    {
      return _dbTransaction.Connection.Query<ApproveListsModel>(@"SELECT
        APT.Id,
        R.ItemGroup,
        R.Unit,
        R.ProductGroup,
        SG.DSGSUBGROUPDESCRIPTION AS SubGroup,
        R.ItemId,
        R.ItemName,
        R.Approved,
        R.Remark,
        E.Name,
        CASE 
          WHEN APS.EmailBackup IS NOT NULL AND APS.EmailBackup <> '' THEN APS.EmailBackup
          ELSE U.Email
        END AS Email,
        APS.ApproveLevel,
        APS.FinalApprove
        FROM ApproveTrans APT
        LEFT JOIN ApproveSetup APS ON APS.Id = APT.ApproveId
        LEFT JOIN DSG_User U ON U.Id = APS.UserId
        LEFT JOIN DSG_Employee E ON E.EmployeeId = U.EmployeeId
        LEFT JOIN RequestTrans R ON R.Id = APT.ItemTransId
        LEFT JOIN [PENTOS\DEVELOP].[AX_Customize].dbo.DSGSubGroup SG ON SG.DSGSUBGROUPID = R.SubGroup AND SG.DATAAREAID = 'dv'
        WHERE APT.SendEmailDate IS NOT NULL
        AND U.Email = @Email
        AND APT.ApproveBy IS NULL
        AND APT.RejectBy IS NULL
        ORDER BY APS.ApproveLevel ASC",
        new { @Email = email },
        _dbTransaction);
    }

    public ApproveTrans GetApproveTrans(int id)
    {
      return _dbTransaction.Connection.Get<ApproveTrans>(id, _dbTransaction);
    }

    public ApproveTransEmailModel GetApproveTransEmail(int itemTransId)
    {
      return _dbTransaction.Connection.Query<ApproveTransEmailModel>(@"SELECT
        APT.Id,
        E.Name,
        CASE 
          WHEN APS.EmailBackup IS NOT NULL AND APS.EmailBackup <> '' THEN APS.EmailBackup
          ELSE U.Email
        END AS Email,
        APS.ApproveLevel,
        APS.FinalApprove
        FROM ApproveTrans APT
        LEFT JOIN ApproveSetup APS ON APS.Id = APT.ApproveId
        LEFT JOIN DSG_User U ON U.Id = APS.UserId
        LEFT JOIN DSG_Employee E ON E.EmployeeId = U.EmployeeId
        WHERE APT.ItemTransId = @ITEMTRANSID
        AND APT.SendEmailDate IS NULL
        ORDER BY APS.ApproveLevel ASC",
        new { @ITEMTRANSID = itemTransId },
        _dbTransaction).FirstOrDefault();
    }

    public bool UpdateApproveTrans(ApproveTrans approveTrans)
    {
      return _dbTransaction.Connection.Update<ApproveTrans>(approveTrans, _dbTransaction);
    }
    public IEnumerable<ApproveTransAlertEditItemModel> GetEmailAlert(int ItemTransId)
    {
        return _dbTransaction.Connection.Query<ApproveTransAlertEditItemModel>(@"SELECT APT.*,CASE 
                          WHEN APS.EmailBackup IS NOT NULL AND APS.EmailBackup <> '' THEN APS.EmailBackup
                          ELSE U.Email
                        END AS Email
                        ,E.Name AS ApproveName
                        ,RQT.RequestBy
                        ,U2.Email AS RequesterEmail
                FROM ApproveTrans APT 
                LEFT JOIN ApproveSetup APS ON APS.Id = APT.ApproveId
                LEFT JOIN DSG_User U ON U.Id = APS.UserId
                LEFT JOIN DSG_Employee E ON E.EmployeeId = U.EmployeeId
                LEFT JOIN RequestTrans RQT ON RQT.Id = APT.ItemTransId
                LEFT JOIN DSG_User U2 ON U2.Id = RQT.RequestBy
                WHERE APT.ItemTransId = @ITEMTRANSID
                AND APT.SendEmailDate IS NOT NULL
                AND APT.RejectDate IS NULL",
             new { @ITEMTRANSID = ItemTransId },
             _dbTransaction);
    }    
  }
}