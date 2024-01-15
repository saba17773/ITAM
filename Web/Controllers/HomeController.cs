using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;
using Web.Domain.Services;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private IPasswordService _passwordService;
    private IConfiguration _configuration;
    private IJwtService _jwtService;
    private IDatabaseContext _databaseContext;
    private IUserService _userService;
    private IEmailService _emailService;

    public HomeController(
      ILogger<HomeController> logger,
      IPasswordService passwordService,
      IConfiguration configuration,
      IJwtService jwtService,
      IDatabaseContext databaseContext,
      IUserService userService,
      IEmailService emailService)
    {
      _logger = logger;
      _passwordService = passwordService;
      _configuration = configuration;
      _jwtService = jwtService;
      _databaseContext = databaseContext;
      _userService = userService;
      _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Index()
    {
      return Redirect("/Item");
    }

    [HttpGet]
    [Authorize]
    public IActionResult Dashboard()
    {
      return View();
    }

    [HttpGet]
    public IActionResult Privacy()
    {
      return View();
    }

    public IActionResult Error(int code)
    {
      if (code == 401)
      {
        return Redirect("/User/Login");
      }
      else
      {
        return BadRequest($"Error code {code};");
      }
    }
  }
}
