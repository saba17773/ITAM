using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Repositories
{
  public class ItemRepository : IItemRepository
  {
    private IDbTransaction _dbTransaction;

    public ItemRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public long AddItem(RequestItem requestItem)
    {
      return _dbTransaction.Connection.Insert<RequestItem>(requestItem, _dbTransaction);
    }

    public IEnumerable<ItemAxModel> GetItemAx(string filter)
    {
      // live [PENTOS\DEVELOP].[AX_Customize]
      // cust [pentos\develop].[AX_Customize]

      string sql = @"SELECT TOP 500
        IT.ITEMGROUPID AS ItemGroup,
        IT.ITEMID AS ItemId,
        IT.ITEMNAME AS ItemName,
        SG.DSGSUBGROUPDESCRIPTION AS SubGroup,
        PG.DSGPRODUCTGROUPDESCRIPTION AS ProductGroup,
        IT.BOMUNITID AS Unit
        FROM [PENTOS\DEVELOP].[AX_Customize].dbo.INVENTTABLE IT
        LEFT JOIN [PENTOS\DEVELOP].[AX_Customize].dbo.DSGSubGroup SG
        ON SG.DSGSUBGROUPID = IT.DSGSUBGROUPID
        AND SG.DATAAREAID = 'dv'
        LEFT JOIN [PENTOS\DEVELOP].[AX_Customize].dbo.DSGProductGroup PG
        ON PG.DSGPRODUCTGROUPID = IT.DSGPRODUCTGROUPID
        AND PG.DATAAREAID = 'dv'
        WHERE SUBSTRING(IT.ITEMGROUPID, 1, 2) NOT IN ('FG', 'RM', 'SM')
        AND IT.DATAAREAID = 'dv'
        AND IT.ITEMNAME  NOT LIKE '%ห้ามใช้%'
        AND " + filter + @"
        
        ORDER BY IT.ITEMID ASC ";

      return _dbTransaction.Connection.Query<ItemAxModel>(sql, null, _dbTransaction);
    }

    public IEnumerable<ItemGroup> GetItemGroup()
    {
      string sql = @"SELECT
        IG.ITEMGROUPID AS Id,
        IG.NAME AS [Description]
        FROM [PENTOS\DEVELOP].[AX_Customize].dbo.INVENTITEMGROUP IG
        WHERE IG.DATAAREAID = 'dv'
        AND SUBSTRING(IG.ITEMGROUPID, 1, 2) NOT IN ('FG', 'RM')";
      return _dbTransaction.Connection.Query<ItemGroup>(sql, null, _dbTransaction);
    }

    public IEnumerable<ItemProductGroup> GetItemProductGroup()
    {
      string sql = @"SELECT
        PG.DSGPRODUCTGROUPID AS Id,
        PG.DSGPRODUCTGROUPDESCRIPTION AS [Description]
        FROM [PENTOS\DEVELOP].[AX_Customize].dbo.DSGProductGroup PG
        WHERE PG.DATAAREAID = 'dv'";
      return _dbTransaction.Connection.Query<ItemProductGroup>(sql, null, _dbTransaction);
    }

    public IEnumerable<RequestItem> GetItems()
    {
      return _dbTransaction.Connection.GetAll<RequestItem>(_dbTransaction);
    }

    public IEnumerable<ItemSubGroup> GetItemSubGroup()
    {
      // string sql = @"SELECT
      //   SG.DSGSUBGROUPID AS Id,
      //   SG.DSGSUBGROUPDESCRIPTION AS [Description]
      //   FROM [PENTOS\DEVELOP].[AX_Customize].dbo.DSGSubGroup SG
      //   WHERE SG.DATAAREAID = 'dv'";

      string sql = @"SELECT 
        SG.DSGSUBGROUPID AS Id,
        SG.DSGSUBGROUPDESCRIPTION AS [Description]
        FROM [PENTOS\DEVELOP].[AX_Customize].dbo.DSGSubGroup SG
        WHERE SG.DSG_ProductGroupID is null or SG.DSG_ProductGroupID = ''
        AND SG.DSG_MKProductGroupId is null or SG.DSG_MKProductGroupId = ''
        AND SG.DSG_MKSubproductGroupId is null or SG.DSG_MKSubproductGroupId = ''
        AND SG.DSG_MKProductTypeId is null or SG.DSG_MKProductTypeId = ''
        AND SG.DATAAREAID = 'dv'";

      return _dbTransaction.Connection.Query<ItemSubGroup>(sql, null, _dbTransaction);
    }

    public IEnumerable<ItemUnit> GetItemUnit()
    {
      string sql = @"SELECT
        U.UNITID AS Id,
        U.TXT AS [Description]
        FROM [PENTOS\DEVELOP].[AX_Customize].dbo.Unit U
        WHERE U.DATAAREAID = 'dv'";
      return _dbTransaction.Connection.Query<ItemUnit>(sql, null, _dbTransaction);
    }

    public IEnumerable<RequestItemGridModel> GetRequestItemGrid(string filter)
    {
      string sql = $@"SELECT TOP 500
        R.Id,
        R.ItemGroup,
        R.Unit,
        R.ProductGroup,
        SG.DSGSUBGROUPID AS SubGroup,
        --SG.DSGSUBGROUPDESCRIPTION AS SubGroup,
        R.ItemId,
        R.ItemName,
        R.Approved,
        R.Remark,
        R.RequestDate,
        U2.Username AS UpdateBy,
        R.UpdateDate,
        R.AxCreated,
        R.AxCreateDate,
        R.AxItemId,
        E.Name + ' ' + E.LastName AS RequestBy
         ,Message = (
		    SELECT T.Message1
		    FROM
		    (
			    SELECT T1.ItemTransId
			    ,Message1 = STUFF((
				    SELECT CASE WHEN T2.Remark = '' OR T2.Remark IS NULL THEN ''
					ELSE  ', ' + T2.Remark END AS Remark 
				    FROM ApproveTrans T2
				    WHERE  T1.ItemTransId = T2.ItemTransId 
				    GROUP BY T2.Remark
			    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')
			    FROM ApproveTrans T1
			    WHERE T1.ItemTransId = R.Id
		    )T
		    GROUP BY T.Message1
	    )
        FROM RequestTrans R
        LEFT JOIN [PENTOS\DEVELOP].[AX_Customize].dbo.DSGSubGroup SG 
        ON SG.DSGSUBGROUPID = R.SubGroup
        AND SG.DATAAREAID = 'dv'
        LEFT JOIN DSG_User U
        ON U.Id = R.RequestBy
        LEFT JOIN DSG_User U2
        ON U2.Id = R.UpdateBy
        LEFT JOIN DSG_Employee E ON E.EmployeeId = U.EmployeeId
        WHERE {filter}
        ORDER BY R.RequestDate DESC";

      return _dbTransaction.Connection.Query<RequestItemGridModel>(sql, null, _dbTransaction);
    }

    public bool UpdateItem(RequestItem requestItem)
    {
      return _dbTransaction.Connection.Update<RequestItem>(requestItem, _dbTransaction);
    }

    public IEnumerable<RequestItemSendMailModel> GetRequestItemSendMail()
    {
      string sql = @"SELECT 
        R.Id,
        R.ItemGroup,
        R.Unit,
        R.ProductGroup,
        SG.DSGSUBGROUPID AS SubGroup,
        --SG.DSGSUBGROUPDESCRIPTION AS SubGroup,
        R.ItemId,
        R.ItemName,
        R.Approved,
        R.Remark,
        R.RequestDate,
        U2.Username AS UpdateBy,
        R.UpdateDate,
        R.AxCreated,
        R.AxCreateDate,
        R.AxItemId,
        E.Name  AS RequestBy,
        IT.ITEMNAME AS AxItemName,
        U.Email EmailRequester
        ,Message = (
		    SELECT T.Message1
		    FROM
		    (
			    SELECT T1.ItemTransId
			    ,Message1 = STUFF((
				    SELECT CASE WHEN T2.Remark = '' OR T2.Remark IS NULL THEN ''
					ELSE  ', ' + T2.Remark END AS Remark 
				    FROM ApproveTrans T2
				    WHERE  T1.ItemTransId = T2.ItemTransId 
				    GROUP BY T2.Remark
			    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')
			    FROM ApproveTrans T1
			    WHERE T1.ItemTransId = R.Id
		    )T
		    GROUP BY T.Message1
	    )
        FROM RequestTrans R
        LEFT JOIN [PENTOS\DEVELOP].[AX_Customize].dbo.DSGSubGroup SG 
        ON SG.DSGSUBGROUPID = R.SubGroup
        AND SG.DATAAREAID = 'dv'
        LEFT JOIN DSG_User U
        ON U.Id = R.RequestBy
        LEFT JOIN DSG_User U2
        ON U2.Id = R.UpdateBy
        LEFT JOIN DSG_Employee E ON E.EmployeeId = U.EmployeeId
        LEFT JOIN [PENTOS\DEVELOP].[AX_Customize].dbo.INVENTTABLE IT 
        ON IT.ITEMID = R.AxItemId AND IT.DATAAREAID = 'dv'
        WHERE R.AxItemId IS NOT NULL 
        AND R.Approved = 1 
        AND R.SendMailItemAX = 2
        ORDER BY R.RequestDate DESC";
            // return _dbTransaction.Connection.GetAll<RequestItem>(_dbTransaction);
        return _dbTransaction.Connection.Query<RequestItemSendMailModel>(sql, null, _dbTransaction);
    }
  }
}