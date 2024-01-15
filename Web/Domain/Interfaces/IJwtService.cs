using Web.Infrastructure.Entities;

namespace Web.Domain.Interfaces
{
  public interface IJwtService
  {
    string GenerateToken(User user);
  }
}