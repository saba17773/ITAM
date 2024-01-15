using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface IApproveService
  {
    List<ApproveResultModel> GetItemApproveResult(IFormCollection formCollection);
    RequestItem GetRequestItemTrans(int id);
    ApproveSetup GetApproveSetup(int id);
    IEnumerable<RequestItemSendMailModel> GetRequestItemTransAlert();
    IEnumerable<ApproveTransAlertEditItemModel> GetEmailAlert(int ItemTransId);
  }
}