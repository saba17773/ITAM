using System.Collections.Generic;
using Web.Infrastructure.Entities;

namespace Web.Domain.Interfaces
{
  public interface IStatusRepository
  {
    IEnumerable<Status> GetStatusAll();
  }
}