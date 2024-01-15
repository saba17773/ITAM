using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;
using Web.Infrastructure.Transfers;
using System.Linq;
using Web.Infrastructure.Entities;

namespace Web.Domain.Services
{
  public class ApproveService : IApproveService
  {
    private IEmailService _emailService;
    private IConfiguration _configuration;
    private IDatabaseContext _databaseContext;

    public ApproveService(
      IEmailService emailService,
      IConfiguration configuration,
      IDatabaseContext databaseContext)
    {
      _emailService = emailService;
      _configuration = configuration;
      _databaseContext = databaseContext;
    }

    public ApproveSetup GetApproveSetup(int id)
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          return unitOfWork.Setup.GetApproveSetup().Where(x => x.Id == id).FirstOrDefault();
        }
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    public List<ApproveResultModel> GetItemApproveResult(IFormCollection formCollection)
    {
      List<ApproveResultModel> result = new List<ApproveResultModel>();
      foreach (var item in formCollection)
      {
        if (item.Key != "email" && item.Key != "nonce")
        {
          var _key = item.Key.Split("_");
          if (_key.Length == 2)
          {
            if (_key[0] == "approve")
            {
              int approveResult = 0;
              string _result = formCollection["approve_" + _key[1].ToString()];
              string _remark = formCollection["message_" + _key[1]].ToString();
              int _transId = Convert.ToInt32(_key[1]);

              if (_result == "")
              {
                approveResult = 0;
              }
              else
              {
                approveResult = Convert.ToInt32(_result);
              }

              result.Add(new ApproveResultModel
              {
                ApproveResult = approveResult,
                Remark = _remark,
                TransId = _transId
              });
            }
          }
        }
      }
      return result;
    }

    public RequestItem GetRequestItemTrans(int id)
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          return unitOfWork.Item.GetItems().Where(x => x.Id == id).FirstOrDefault();
        }
      }
      catch (System.Exception)
      {

        throw;
      }

    }

   public IEnumerable<RequestItemSendMailModel> GetRequestItemTransAlert()
   {
        try
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                    return unitOfWork.Item.GetRequestItemSendMail();
            }
        }
        catch (System.Exception)
        {
            throw;
        }

   }
    public IEnumerable<ApproveTransAlertEditItemModel> GetEmailAlert(int ItemTransId)
    {
        try
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                return unitOfWork.Approve.GetEmailAlert(ItemTransId);
            }
        }
        catch (System.Exception)
        {
            throw;
        }
            
    }
  }
}