using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class ResponseModel
  {
    [Required]
    public bool Result { get; set; }
    public string Message { get; set; }
  }
}