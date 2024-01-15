using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Web.Domain.Interfaces;

namespace Web.Infrastructure.Contexts
{
  public class DatabaseContext : IDatabaseContext
  {
    private IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IDbConnection GetConnection(string connString = null)
    {
      if (connString == null)
      {
        return new SqlConnection(_configuration["Database:Default"]);
      }
      else
      {
        return new SqlConnection(_configuration["Database:" + connString]);
      }
    }
  }
}