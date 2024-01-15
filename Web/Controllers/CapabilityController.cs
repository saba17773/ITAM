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
  public class CapabilityController : Controller
  {
    private IDatabaseContext _databaseContext;
    private IDatatableService _datatableService;
    private IMessageService _messageService;

    public CapabilityController(
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
    public IActionResult GetCapabilityAllGrid()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var capability = unitOfWork.Capability.GetCapabilityAll().ToList();
          unitOfWork.Complete();
          return Json(_datatableService.FormatOnce(capability));
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    public ResponseModel UpdateCapabilityGrid(UpdateGridModel updateGrid)
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
          var updateCapability = unitOfWork.Capability.UpdateCapabilityGrid(updateGrid);
          if (updateCapability == -1)
          {
            throw new Exception("Update " + updateGrid.name + " failed.");
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Update capability success."
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
    public ResponseModel AddCapability(AddCapabilityModel addCapability)
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
          var capability = new Capability
          {
            Description = addCapability.CapabilityDescription,
            Slug = addCapability.Slug
          };

          var newCapability = unitOfWork.Capability.AddCapability(capability);
          if (newCapability == -1)
          {
            throw new Exception("Add capability failed.");
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Add capability success."
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

    // end
  }
}