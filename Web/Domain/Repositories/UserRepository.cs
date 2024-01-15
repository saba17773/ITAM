using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IDbTransaction _dbTransaction;

        public UserRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public long AddUser(User user)
        {
            return _dbTransaction.Connection.Insert<User>(user, _dbTransaction);
        }

        public User GetUser(int userId)
        {
            return _dbTransaction.Connection.Get<User>(userId, _dbTransaction);
        }

        public IEnumerable<User> GetUserAll()
        {
            return _dbTransaction.Connection.GetAll<User>(_dbTransaction);
        }

        public IEnumerable<UserGridModel> GetUserGrid(string filter)
        {
            return _dbTransaction.Connection.Query<UserGridModel>(@"
        SELECT
          U.Id,
          U.Username,
          U.EmployeeId,
          E.Name,
          E.LastName,
          E.DivisionName,
          E.DepartmentName,
          R.[Description] AS RoleName,
          U.Email,
          S.[Description] AS Active, 
          U.Remark,
          E.Company
        FROM DSG_User U
        LEFT JOIN DSG_Employee E
        ON E.EmployeeId = U.EmployeeId 
        AND E.Status = 3
        LEFT JOIN DSG_Role R ON R.Id = U.RoleId
        LEFT JOIN DSG_Status S ON S.Id = U.Active
        WHERE " + filter + @"
        ORDER BY U.Active DESC
      ", null, _dbTransaction);
        }

        public bool UpdateUser(User user)
        {
            return _dbTransaction.Connection.Update<User>(user, _dbTransaction);
        }

        public int UpdateUserGrid(UpdateGridModel updateGrid)
        {
            return _dbTransaction.Connection.Execute(@"
        UPDATE DSG_User
        SET " + updateGrid.name + @" = @Value
        WHERE Id = @Id
      ", new
            {
                @Value = updateGrid.value,
                @Id = updateGrid.pk
            }, _dbTransaction);
        }

        public User ValidateUser(UserLoginModel userLogin)
        {
            return _dbTransaction.Connection
              .GetAll<User>(_dbTransaction)
              .Where(o =>
                o.Username == userLogin.Username.Trim() &&
                o.Password == userLogin.Password)
              .FirstOrDefault();
        }

        public long AddLogUser(UserLoginModel userLogin)
        {
            string strHostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(strHostName).GetValue(1).ToString();

            return _dbTransaction.Connection.Execute(@"
        INSERT INTO [EA_APP].[dbo].[TB_LOG_APP] (EMP_CODE,USER_NAME,HOST_NAME," + userLogin.Type + @",PROJECT_NAME)
        VALUES (@EmployeeId,@Username,@hostname,@datenow,@projectname)
      ", new
            {
                @EmployeeId = userLogin.EmployeeId,
                @Username = userLogin.Username,
                @hostname = clientIPAddress,
                @datenow = DateTime.Now,
                @projectname = "Create Item"
            }, _dbTransaction);
        }

        public long AddLogUserApp(User user)
        {
            string strHostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(strHostName).GetValue(1).ToString();

            return _dbTransaction.Connection.Execute(@"
        INSERT INTO [EA_APP].[dbo].[TB_USER_APP] (EMP_CODE,USER_NAME,HOST_NAME,CREATE_DATE,PROJECT_NAME)
        VALUES (@EmployeeId,@Username,@hostname,@datenow,@projectname)
      ", new
            {
                @EmployeeId = user.EmployeeId,
                @Username = user.Username,
                @hostname = clientIPAddress,
                @datenow = DateTime.Now,
                @projectname = "Create Item"
            }, _dbTransaction);
        }

        public long DeleteLogUser(User user)
        {
            return _dbTransaction.Connection.Execute(@"
                DELETE FROM [EA_APP].[dbo].[TB_USER_APP]
                WHERE EMP_CODE = @employeeid AND USER_NAME = @username AND PROJECT_NAME = @projectname
              ", new
            {
                @employeeid = user.EmployeeId,
                @username = user.Username,
                @projectname = "Create Item"
            }, _dbTransaction);
        }

        public long UpdateLogUser(User user)
        {
            return _dbTransaction.Connection.Execute(@"
                UPDATE [MORMONT\DEVELOP].[EA_APP].[dbo].[TB_USER_APP] 
	            SET UPDATE_DATE = GETDATE(), STATUS = 0
	            WHERE EMP_CODE = @employeeid AND  USER_NAME= @username AND PROJECT_NAME = @projectname
	            IF @@ROWCOUNT = 0
	            BEGIN
		            INSERT INTO [MORMONT\DEVELOP].[EA_APP].[dbo].[TB_USER_APP] (
			            EMP_CODE, 
			            [HOST_NAME], 
			            [USER_NAME], 
			            PROJECT_NAME, 
			            CREATE_DATE, 
			            UPDATE_DATE, 
			            [STATUS]
		            ) VALUES (
			            @employeeid, '-', @username, @projectname, GETDATE(),
			            GETDATE(), 0
		            )
	            END", new
            {
                @employeeid = user.EmployeeId,
                @username = user.Username,
                @projectname = "Create Item"
            }, _dbTransaction);
        }
    }

}