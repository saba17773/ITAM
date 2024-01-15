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
  public class RoleController : Controller
  {
    private IDatabaseContext _databaseContext;
    private IDatatableService _datatableService;
    private IMessageService _messageService;

    public RoleController(
      IDatabaseContext databaseContext,
      IDatatableService datatableService,
      IMessageService messageService)
    {
      _databaseContext = databaseContext;
      _datatableService = datatableService;
      _messageService = messageService;
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetRoleAllActive()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var roles = unitOfWork.Role.GetRoleAll()
             .Where(o => o.Active == 1)
             .ToList();

          unitOfWork.Complete();

          return Json(roles);
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetRoleAll()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var roles = unitOfWork.Role.GetRoleAll().ToList();

          unitOfWork.Complete();

          return Json(roles);
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    public IActionResult GetRoleAllGrid()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var roles = unitOfWork.Role.GetRoleAllGrid().ToList();

          unitOfWork.Complete();

          return Json(_datatableService.FormatOnce(roles));
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    public ResponseModel AddRole(AddRoleModel addRole)
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
          var role = new Role { Description = addRole.RoleDescription, Active = 1 };

          var newRole = unitOfWork.Role.AddRole(role);
          if (newRole == -1)
          {
            throw new Exception("Add role failed.");
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Add role success."
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

    [HttpPost]
    [Authorize]
    public ResponseModel UpdateRoleGrid(UpdateGridModel updateGrid)
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
          var updateUser = unitOfWork.Role.UpdateRoleGrid(updateGrid);
          if (updateUser == -1)
          {
            throw new Exception("Update " + updateGrid.name + " failed.");
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Update role success."
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

    // End
  }
}