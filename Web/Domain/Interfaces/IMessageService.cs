using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Domain.Interfaces
{
  public interface IMessageService
  {
    List<string> ErrorMessage(ModelStateDictionary modelState);
  }
}