using System.Collections.Generic;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Interfaces
{
  public interface IItemRepository
  {
    IEnumerable<ItemGroup> GetItemGroup();
    IEnumerable<ItemUnit> GetItemUnit();
    IEnumerable<ItemProductGroup> GetItemProductGroup();
    IEnumerable<ItemSubGroup> GetItemSubGroup();
    long AddItem(RequestItem requestItem);
    bool UpdateItem(RequestItem requestItem);
   // long CancelItem(RequestItem requestItem);
    IEnumerable<RequestItem> GetItems();
    IEnumerable<RequestItemGridModel> GetRequestItemGrid(string filter);
    IEnumerable<ItemAxModel> GetItemAx(string filter);
    IEnumerable<RequestItemSendMailModel> GetRequestItemSendMail();

    }
}