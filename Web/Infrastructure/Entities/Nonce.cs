using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
  [Table("DSG_Nonce")]
  public class Nonce
  {
    public int Id { get; set; }
    public string NonceKey { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int Used { get; set; }
  }
}