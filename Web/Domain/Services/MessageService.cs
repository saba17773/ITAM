using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Domain.Interfaces;

namespace Web.Domain.Services
{
  public class MessageService : IMessageService
  {
    public List<string> ErrorMessage(ModelStateDictionary modelState)
    {
      try
      {
        List<string> errorMessage = new List<string>();

        foreach (var value in modelState.Values)
        {
          foreach (var err in value.Errors)
          {
            errorMessage.Add(err.ErrorMessage);
          }
        }

        return errorMessage;
      }
      catch (System.Exception)
      {

        throw;
      }
    }
  }
}