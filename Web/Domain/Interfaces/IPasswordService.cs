using System.Security.Authentication;

namespace Web.Domain.Interfaces
{
  public interface IPasswordService
  {
    string HashPassword(string password);
  }
}