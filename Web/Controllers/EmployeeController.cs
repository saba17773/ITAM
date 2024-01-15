using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;

namespace Web.Controllers
{
  public class EmployeeController : Controller
  {
    private IDatabaseContext _databaseContext;
    private IDatatableService _datatableService;

    public EmployeeController(IDatabaseContext databaseContext, IDatatableService datatableService)
    {
      _databaseContext = databaseContext;
      _datatableService = datatableService;
    }

    [HttpPost]
    [Authorize]
    public IActionResult GetEmployeeAll()
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var employee = unitOfWork.Employee.GetEmployeeAll().ToList();
          unitOfWork.Complete();
          return Json(_datatableService.FormatOnce(employee));
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }
  }
}