using System;
using System.Security.Authentication;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Web.Domain.Interfaces;

namespace Web.Domain.Services
{
  public class PasswordService : IPasswordService
  {
    private IConfiguration _configuration;

    public PasswordService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public string HashPassword(string password)
    {
      string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: Encoding.ASCII.GetBytes(_configuration["Config:SecretKey"]),
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));
      return hashed;
    }
  }
}