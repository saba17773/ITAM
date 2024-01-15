using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Web.Domain.Interfaces
{
  public interface IDatatableService
  {
    object Format<T>(HttpRequest req, List<T> data);
    string Filter(HttpRequest req, object field);
    object FormatOnce<T>(List<T> data);
    string GetField(string column, object field);
  }
}