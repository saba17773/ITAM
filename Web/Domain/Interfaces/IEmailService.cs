using System.Collections.Generic;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface IEmailService
  {
    ResponseModel SendEmail(string subject, string htmlBody, List<string> mailTo, string sender = "", string replyTo = "");
  }
}