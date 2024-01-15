namespace Web.Domain.Interfaces
{
  public interface IItemService
  {
    string GenerateItemId(string prefix, int number);
  }
}