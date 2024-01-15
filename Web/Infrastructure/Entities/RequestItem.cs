using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("RequestTrans")]
  public class RequestItem
  {
    public int Id { get; set; }
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public string ItemGroup { get; set; }
    public string Unit { get; set; }
    public string ProductGroup { get; set; }
    public string SubGroup { get; set; }
    public int Approved { get; set; }
    public string Remark { get; set; }
    public int RequestBy { get; set; }
    public DateTime RequestDate { get; set; }
    public int? UpdateBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? AxCreated { get; set; }
    public DateTime? AxCreateDate { get; set; }
    public string AxItemId { get; set; }
    public int SendMailItemAX { get; set; }
  }
}