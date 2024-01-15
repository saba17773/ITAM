using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class ApproveListsModel
  {
    public int Id { get; set; }
    public string ItemGroup { get; set; }
    public string Unit { get; set; }
    public string ProductGroup { get; set; }
    public string SubGroup { get; set; }
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public int Approved { get; set; }
    public string Remark { get; set; }
    public string Name { get; set; }

    [EmailAddress]
    public string Email { get; set; }
    public int ApproveLevel { get; set; }
    public int FinalApprove { get; set; }
  }
}