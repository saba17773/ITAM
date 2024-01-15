using System;
using System.Collections.Generic;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Web.Domain.Interfaces;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Services
{
  public class EmailService : IEmailService
  {
    private IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public ResponseModel SendEmail(string subject, string htmlBody, List<string> mailTo, string sender = "", string replyTo = "")
    {
      try
      {
        var message = new MimeMessage();
        var bodyBuilder = new BodyBuilder();

        if (sender == "")
        {
          sender = _configuration["Email:Username"];
        }

        message.From.Add(new MailboxAddress(sender, sender));
        message.Sender = new MailboxAddress(_configuration["Email:Username"], _configuration["Email:Username"]);

        if (mailTo.Count > 0)
        {
          foreach (var mail in mailTo)
          {
            message.To.Add(new MailboxAddress(mail, mail));
          }
        }
        else
        {
          throw new Exception("No email to send to.");
        }

        if (replyTo == "")
        {
          message.ReplyTo.Add(new MailboxAddress(replyTo, replyTo));
        }

        message.Subject = subject;
        bodyBuilder.HtmlBody = htmlBody;
        message.Body = bodyBuilder.ToMessageBody();

        var client = new SmtpClient();

        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
        client.Connect(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Port"]), SecureSocketOptions.SslOnConnect);
        client.Authenticate(_configuration["Email:Username"], _configuration["Email:Password"]);
        client.Send(message);
        client.Disconnect(true);

        return new ResponseModel
        {
          Result = true,
          Message = "Send mail success."
        };
      }
      catch (System.Exception ex)
      {
        return new ResponseModel
        {
          Result = false,
          Message = ex.Message
        };
      }
    }
  }
}