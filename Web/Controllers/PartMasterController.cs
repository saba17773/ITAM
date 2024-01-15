using System;
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
  public class PartMasterController : Controller
  {
    private IUserService _userService;
    private IDatatableService _datatabaseService;
    private IDatabaseContext _databaseContext;
    private IMessageService _messageService;

    public PartMasterController(
      IUserService userService,
      IDatatableService datatableService,
      IDatabaseContext databaseContext,
      IMessageService messageService)
    {
      _userService = userService;
      _datatabaseService = datatableService;
      _databaseContext = databaseContext;
      _messageService = messageService;
    }

    [Authorize]
    public IActionResult Index()
    {
      _userService.CanAccess("menu-part-master");
      return View("~/Views/Master/PartMaster.cshtml");
    }

    [Authorize]
    [HttpPost]
    public IActionResult GetPartMasterGrid()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var data = unitOfWork.PartMaster.GetPartMaster().ToList();
          return Json(_datatabaseService.FormatOnce(data));
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    public ResponseModel UpdatePartMasterGrid(UpdateGridModel updateGrid)
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
          var checkDuplicate =
           unitOfWork.PartMaster
           .GetPartMaster()
           .Where(x => x.PartName == updateGrid.value && x.Active == 1)
           .FirstOrDefault();

          if (checkDuplicate != null)
          {
            throw new Exception("Part Name ใช้งานอยู่แล้ว");
          }

          var updateUser = unitOfWork.PartMaster.UpdatePartMasterGrid(updateGrid);
          if (updateUser == -1)
          {
            throw new Exception("Update " + updateGrid.name + " failed.");
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Update User Success."
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
    public ResponseModel Add(FormAddPartNameModel formAddPartName)
    {
      try
      {
        var errMessage = _messageService.ErrorMessage(ModelState);
        if (errMessage.Count > 0)
        {
          return new ResponseModel
          {
            Result = false,
            Message = errMessage[0]
          };
        }

        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var checkDuplicate =
            unitOfWork.PartMaster
            .GetPartMaster()
            .Where(x => x.PartName == formAddPartName.PartName && x.Active == 1)
            .FirstOrDefault();

          if (checkDuplicate != null)
          {
            throw new Exception("Part Name ใช้งานอยู่แล้ว");
          }

          unitOfWork.PartMaster.AddPartMaster(new PartMaster { PartName = formAddPartName.PartName });
          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Add Part Name Success."
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
    public IActionResult GetPartMaster()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          return Json(unitOfWork.PartMaster.GetPartMaster().Where(x => x.Active == 1).ToList());
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }
  } // end class
}