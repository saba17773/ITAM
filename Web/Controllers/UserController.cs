using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;
using Web.Domain.Services;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Controllers
{
  public class UserController : Controller
  {
    private IPasswordService _passwordService;
    private IDatabaseContext _databaseContext;
    private IMessageService _messageService;
    private IJwtService _jwtService;
    private IConfiguration _configuration;
    private IDatatableService _datatableService;
    private IUserService _userService;

    public UserController(
      IPasswordService passwordService,
      IDatabaseContext databaseContext,
      IMessageService messageService,
      IJwtService jwtService,
      IConfiguration configuration,
      IDatatableService datatableService,
      IUserService userService)
    {
      _passwordService = passwordService;
      _databaseContext = databaseContext;
      _messageService = messageService;
      _jwtService = jwtService;
      _configuration = configuration;
      _datatableService = datatableService;
      _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
      _userService.CanAccess("menu-user-management");
      return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
      return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
      return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
      try
      {
        // Response.Cookies.Delete(_configuration["Jwt:Name"]);
        // return Redirect("/User/Login");
        return Redirect("/User/AddLog?type=LOGOUT_DATE");
        // ViewData["SuccessMessage"] = new List<string>() { "Logout success." };
        // return View("~/Views/User/Login.cshtml");
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ResponseModel AddUser(User user)
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
          var currentUser = unitOfWork.User.GetUserAll()
            .Where(o => o.Username == user.Username.Trim())
            .FirstOrDefault();

          if (currentUser != null)
          {
            throw new Exception("User \"" + user.Username + "\" already exists.");
          }

          user.Password = _passwordService.HashPassword(user.Password);
          long addUser = unitOfWork.User.AddUser(user);
          if (addUser == -1)
          {
            throw new Exception("Add user failed.");
          }
          if(user.Active==1){
            var applog = new User
            {
                EmployeeId = user.EmployeeId,
                Username = user.Username
            };
            var newLog = unitOfWork.User.AddLogUserApp(applog);
            if (newLog == -1)
            {
                throw new Exception("Inset App Log failed.");
            }
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Add user success."
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
    public IActionResult Auth(UserLoginModel userLogin)
    {
      try
      {
        var errMessage = _messageService.ErrorMessage(ModelState);
        if (errMessage.Count > 0)
        {
          ViewData["ErrorMessage"] = errMessage;
          return View("~/Views/User/Login.cshtml");
        }

        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          userLogin.Password = _passwordService.HashPassword(userLogin.Password);

          var user = unitOfWork.User.ValidateUser(userLogin);
          if (user == null)
          {
            throw new Exception("User not found.");
          }

          string token = _jwtService.GenerateToken(user);

          Response.Cookies.Append(_configuration["Jwt:Name"], token, new CookieOptions()
          {
            Expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:Exp"])),
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
          });

          // var applog = new UserLoginModel
          // {
          //     EmployeeId = Convert.ToInt32(_userService.GetClaim().EmployeeId),
          //     Username = _userService.GetClaim().Username.ToString()
          // };
          // var newLog = unitOfWork.User.AddLogUser(applog);
          // if (newLog == -1)
          // {
          //     throw new Exception("Inset App Log failed.");
          // }
          // unitOfWork.Complete();

        }
        
        // ViewData["SuccessMessage"] = new List<string>() { "Login success."};
        
        return Redirect("/User/AddLog?type=LOGIN_DATE");
      }
      catch (System.Exception ex)
      {
        ViewData["ErrorMessage"] = new List<string>() { ex.Message };

        return View("~/Views/User/Login.cshtml");
      }
    }

    [HttpPost]
    [Authorize]
    public IActionResult UserGrid()
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

          var userGrid = unitOfWork.User.GetUserGrid(filter);

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

    [HttpPost]
    [Authorize]
    public ResponseModel UpdateUserGrid(UpdateGridModel updateGrid)
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
          var updateUser = unitOfWork.User.UpdateUserGrid(updateGrid);
          if (updateUser == -1)
          {
            throw new Exception("Update " + updateGrid.name + " failed.");
          }

          var currentUser = unitOfWork.User.GetUserAll()
            .Where(o => o.Id == updateGrid.pk)
            .FirstOrDefault();
          // Console.WriteLine(currentUser.Username);

          if(updateGrid.name=="Active" && updateGrid.value=="2"){
            var applog = new User
            {
                EmployeeId = currentUser.EmployeeId,
                Username = currentUser.Username
            };
            var newLog = unitOfWork.User.UpdateLogUser(applog);
            if (newLog == -1)
            {
                throw new Exception("Update App Log failed.");
            }
          }

          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Update user success."
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
    [ValidateAntiForgeryToken]
    public IActionResult ChangePasswordSave(ChangePasswordModel changePassword)
    {
      try
      {
        var errMessage = _messageService.ErrorMessage(ModelState);
        if (errMessage.Count > 0)
        {
          throw new Exception(errMessage[0]);
        }

        UserClaimModel userClaim = _userService.GetClaim();

        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          User user = unitOfWork.User.GetUser(userClaim.UserId);
          if (user.Password != _passwordService.HashPassword(changePassword.OldPassword))
          {
            throw new Exception("Old Password incorrect.");
          }

          user.Password = _passwordService.HashPassword(changePassword.NewPassword);
          unitOfWork.User.UpdateUser(user);
          unitOfWork.Complete();
        }

        ViewData["SuccessMessage"] = new List<string>() { "Update password success." };
        return View("~/Views/User/ChangePassword.cshtml");
      }
      catch (System.Exception ex)
      {
        ViewData["ErrorMessage"] = new List<string>() { ex.Message };
        return View("~/Views/User/ChangePassword.cshtml");
      }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ResponseModel ResetPassword(ResetPasswordModel resetPassword)
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
          var user = unitOfWork.User.GetUser(resetPassword.UserId);
          user.Password = _passwordService.HashPassword(resetPassword.NewPassword);
          unitOfWork.User.UpdateUser(user);
          unitOfWork.Complete();
        }

        return new ResponseModel
        {
          Result = true,
          Message = "Reset password success."
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

    [HttpGet]
    public IActionResult AddLog(UserLoginModel userLogin)
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          
          if(userLogin.Type == "LOGIN_DATE"){
            var applog = new UserLoginModel
            {
                EmployeeId = Convert.ToInt32(_userService.GetClaim().EmployeeId),
                Username = _userService.GetClaim().Username.ToString(),
                Type = "LOGIN_DATE"
            };
            var newLog = unitOfWork.User.AddLogUser(applog);
            if (newLog == -1)
            {
                throw new Exception("Inset App Log failed.");
            }
          }else{
            var applog = new UserLoginModel
            {
                EmployeeId = Convert.ToInt32(_userService.GetClaim().EmployeeId),
                Username = _userService.GetClaim().Username.ToString(),
                Type = "LOGOUT_DATE"
            };
            var newLog = unitOfWork.User.AddLogUser(applog);
            if (newLog == -1)
            {
                throw new Exception("Inset App Log failed.");
            }
          }
          
          unitOfWork.Complete();
        }
        
        if(userLogin.Type == "LOGIN_DATE"){
          ViewData["SuccessMessage"] = new List<string>() { "Login success."};
          return Redirect("/");
        }else{
          Response.Cookies.Delete(_configuration["Jwt:Name"]);
          ViewData["SuccessMessage"] = new List<string>() { "Logout success."};
          return View("~/Views/User/Login.cshtml");
          
        }
      }
      catch (System.Exception)
      {
          
          throw;
      }
    }
    // End
  }
}