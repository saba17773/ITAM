using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Services
{
  public class UserService : IUserService
  {
    private IHttpContextAccessor _httpContextAccessor;
    private IDatabaseContext _databaseContext;

    public UserService(
      IHttpContextAccessor httpContextAccessor,
      IDatabaseContext databaseContext)
    {
      _httpContextAccessor = httpContextAccessor;
      _databaseContext = databaseContext;
    }

    public void CanAccess(string slug)
    {
      try
      {
        var userCanDisplay = CanDisplay(slug);
        if (userCanDisplay == false)
        {
          throw new Exception("You can't access this section.");
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    public bool CanDisplay(string slug)
    {
      try
      {
        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated == false)
        {
          throw new Exception("Please login.");
        }

        int roleId = GetClaim().RoleId;
        int userId = GetClaim().UserId;

        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var capability = unitOfWork.Capability.GetCapabilityAll()
            .Where(o => o.Slug == slug)
            .FirstOrDefault();

          if (capability == null)
          {
            throw new Exception("Permission incorrect.");
          }

          var user = unitOfWork.User.GetUser(userId);

          var canAccess = unitOfWork.Permission.GetPermissionAll()
            .Where(o => o.RoleId == roleId && o.CapabilityId == capability.Id)
            .FirstOrDefault();

          unitOfWork.Complete();

          if (canAccess == null)
          {
            if (user.Username == "admin")
            {
              return true;
            }
            return false;
          }
          else
          {
            return true;
          }
        }
      }
      catch (System.Exception)
      {
        return false;
      }
    }

    public UserClaimModel GetClaim()
    {
      var _claim = new UserClaimModel
      {
        UserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(
          o => o.Type.ToString().Equals("id")).Value),
        RoleId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(
          o => o.Type.ToString().Equals("role_id")).Value),
        Username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(
          o => o.Type.ToString().Equals("username")).Value,
        EmployeeId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(
          o => o.Type.ToString().Equals("employee_id")).Value
      };
      return _claim;
    }
  }
}