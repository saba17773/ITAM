namespace Web.Domain.Interfaces
{
  public interface IHelperService
  {
    object GetPropertyValue(object src, string propName);
  }
}