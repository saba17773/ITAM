using Web.Infrastructure.Transfers;

namespace Web.Domain.Services
{
  public interface IUserService
  {
    UserClaimModel GetClaim();
    bool CanDisplay(string slug);
    void CanAccess(string slug);
  }
}