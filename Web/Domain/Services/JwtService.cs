using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Web.Domain.Interfaces;
using Web.Infrastructure.Entities;

namespace Web.Domain.Services
{
  public class JwtService : IJwtService
  {

    private IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
      try
      {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new Claim[]
        {
          new Claim("id", user.Id.ToString()),
          new Claim("role_id", user.RoleId.ToString()),
          new Claim("username", user.Username),
          new Claim("employee_id", user.EmployeeId)
        };

        var token = new JwtSecurityToken(
          _configuration["Jwt:Issuer"],
          _configuration["Jwt:Issuer"],
          claims,
          expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:Exp"])),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
      }
      catch (System.Exception)
      {

        throw;
      }
    }
  }
}