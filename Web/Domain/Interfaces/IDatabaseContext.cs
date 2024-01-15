using System.Data;

namespace Web.Domain.Interfaces
{
  public interface IDatabaseContext
  {
    IDbConnection GetConnection(string connString = null);
  }
}