using System.Collections.Generic;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
    public interface IUserRepository
    {
        long AddUser(User user);
        User ValidateUser(UserLoginModel userLogin);
        IEnumerable<UserGridModel> GetUserGrid(string filter);
        int UpdateUserGrid(UpdateGridModel updateGrid);
        User GetUser(int userId);
        IEnumerable<User> GetUserAll();
        bool UpdateUser(User user);
        long AddLogUser(UserLoginModel userLogin);
        long AddLogUserApp(User user);
        long DeleteLogUser(User user);
        long UpdateLogUser(User user);
    }
}