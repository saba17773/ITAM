using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("ApproveTrans")]
  public class ApproveTrans
  {
    public int Id { get; set; }

    [Required]
    public int ApproveId { get; set; }

    [Required]
    public int ItemTransId { get; set; }
    public int? ApproveBy { get; set; }
    public DateTime? ApproveDate { get; set; }
    public int? RejectBy { get; set; }
    public DateTime? RejectDate { get; set; }
    public DateTime? SendEmailDate { get; set; }
    public string Remark { get; set; }
  }
}