using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
    public class CancelItemModel
    {
        [Required]
        public int Id { get; set; }
        public int Approved { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        
    }
}
