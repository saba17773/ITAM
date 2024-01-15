using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Controllers
{
  public class PermissionController : Controller
  {
    public IDatabaseContext _databaseContext;
    public IDatatableService _datatableService;
    private IMessageService _messageService;

    public PermissionController(
      IDatabaseContext databaseContext,
      IDatatableService datatableService,
      IMessageService messageService)
    {
      _databaseContext = databaseContext;
      _datatableService = datatableService;
      _messageService = messageService;
    }

    [HttpPost]
    [Authorize]
    public IActionResult GetPermission(int roleId)
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var permission = unitOfWork.Permission.GetPermissionByRole(roleId).ToList();
          unitOfWork.Complete();
          return Json(_datatableService.FormatOnce(permission));
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    public ResponseModel AddPermission(Permission permission)
    {
      try
      {
        var errMessage = _messageService.ErrorMessage(ModelState);
        if (errMessage.Count > 0)
        {
          throw new Exception(errMessage[0]);
        }

        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var isAdded = unitOfWork.Permission.GetPermissionAll()
            .Where(o => o.RoleId == permission.RoleId && o.CapabilityId == permission.CapabilityId)
            .FirstOrDefault();

          if (isAdded == null)
          {
            var addPermission = unitOfWork.Permission.AddPermission(permission);
            if (addPermission == -1)
            {
              throw new Exception("Add permission failed.");
            }
          }
          else
          {
            var deletePermission = unitOfWork.Permission.DeletePermission(isAdded);
            if (deletePermission == false)
            {
              throw new Exception("Delete permission failed.");
            }
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Update permission success."
        };
      }
      catch (System.Exception ex)
      {
        return new ResponseModel
        {
          Result = false,
          Message = ex.Message
        };
      }
    }
  }
}