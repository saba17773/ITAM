using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Infrastructure.Transfers
{
    public class ApproveTransAlertEditItemModel
    {
        public int Id { get; set; }
        public int ApproveId { get; set; }
        public int ItemTransId { get; set; }
        public int ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public int RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public string Remark { get; set; }
        public string Email { get; set; }
        public string RequesterEmail { get; set; }
        public string ApproveName { get; set; }
    }
}
