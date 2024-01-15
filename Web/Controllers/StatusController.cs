using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;

namespace Web.Controllers
{
  public class StatusController : Controller
  {
    private IDatabaseContext _databaseContext;

    public StatusController(IDatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    [HttpGet]
    [Authorize]
    public JsonResult GetStatusAll()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var status = unitOfWork.Status.GetStatusAll().ToList();
          unitOfWork.Complete();
          return Json(status);
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }
  }
}