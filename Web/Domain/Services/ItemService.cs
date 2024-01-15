using Web.Domain.Interfaces;

namespace Web.Domain.Services
{
  public class ItemService : IItemService
  {
    public string GenerateItemId(string prefix, int number)
    {
      return prefix + "-" + number.ToString().PadLeft(7, '0');
    }
  }
}