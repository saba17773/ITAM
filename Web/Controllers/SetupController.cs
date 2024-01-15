using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;
using Web.Domain.Services;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Controllers
{
  public class SetupController : Controller
  {
    private IUserService _userService;
    private IDatabaseContext _databaseContext;
    private IMessageService _messageService;
    private IDatatableService _datatableService;

    public SetupController(
      IUserService userService,
      IDatabaseContext databaseContext,
      IMessageService messageService,
      IDatatableService datatableService)
    {
      _userService = userService;
      _databaseContext = databaseContext;
      _messageService = messageService;
      _datatableService = datatableService;
    }

    public IActionResult Index()
    {
      try
      {
        return View();
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetApproveGroup()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var group = unitOfWork.Approve.GetApproveGroup();
          unitOfWork.Complete();
          return Json(group.ToList());
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    public ResponseModel AddFlowApprove(FormAddApproveGroupModel formAddApproveGroup)
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
          var addGroup = unitOfWork.Setup.AddApproveSetup(new ApproveSetup
          {
            UserId = formAddApproveGroup.UserId,
            ApproveGroup = formAddApproveGroup.GroupId
          });

          unitOfWork.Complete();

          if (addGroup == -1)
          {
            throw new Exception("Add Approve Setup Failed.");
          }
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Add Approve Setup Success."
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
    public IActionResult GetApproveSetupGrid()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var approveSetupGrid = unitOfWork.Setup.GetApproveSetupGrid();
          unitOfWork.Complete();

          return Json(_datatableService.FormatOnce(approveSetupGrid.ToList()));
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    public ResponseModel UpdateApproveSetupGrid(UpdateGridModel updateGrid)
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
          var updateApprove = unitOfWork.Setup.UpdateApproveSetupGrid(updateGrid);
          if (updateApprove == -1)
          {
            throw new Exception("Update " + updateGrid.name + " failed.");
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Update Success."
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
    public ResponseModel UpdateFlowUser(int userId, int id)
    {
      try
      {

        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var row = unitOfWork.Setup
            .GetApproveSetup()
            .Where(x => x.Id == id)
            .FirstOrDefault();

          if (row == null)
          {
            throw new Exception("Record " + id + " not found.");
          }

          row.UserId = userId;

          var update = unitOfWork.Setup.UpdateApproveSetup(row);

          if (!update)
          {
            throw new Exception("Update failed.");
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Update Success"
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
    public IActionResult GetApproveGroupGrid()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var data = unitOfWork.Setup.GetApproveGroup().ToList();
          return Json(_datatableService.FormatOnce(data));
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    public IActionResult ChangeUserGrid()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var filter = _datatableService.Filter(
            HttpContext.Request,
            new
            {
              Id = "U.Id",
              Username = "U.Username",
              EmployeeId = "U.EmployeeId",
              Name = "E.Name",
              LastName = "E.LastName",
              DivisionName = "E.DivisionName",
              DepartmentName = "E.DepartmentName",
              RoleName = "R.Description",
              Active = "S.Description",
              Email = "E.Email"
            });

          var userGrid = unitOfWork.User.GetUserGrid(filter).Where(x => x.Active == "Active");

          unitOfWork.Complete();

          return Json(_datatableService.Format(
            HttpContext.Request,
            userGrid.OrderBy(o => o.Id).ToList()));
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }
  }
}