using System.Collections.Generic;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface IApproveRepository
  {
    IEnumerable<ApproveGroup> GetApproveGroup();
    long GenerateApproveTrans(IEnumerable<ApproveTrans> approveTrans);
    ApproveTransEmailModel GetApproveTransEmail(int itemTransId);
    ApproveTrans GetApproveTrans(int id);
    bool UpdateApproveTrans(ApproveTrans approveTrans);
    IEnumerable<ApproveListsModel> GetApproveLists(string email);
    IEnumerable<ApproveTransAlertEditItemModel> GetEmailAlert(int ItemTransId);
  }
}