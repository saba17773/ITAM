using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Infrastructure.Transfers
{
    public class RequestItemSendMailModel
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
        public string RequestBy { get; set; }
        public DateTime? RequestDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? AxCreated { get; set; }
        public DateTime? AxCreateDate { get; set; }
        public string AxItemId { get; set; }
        public string Message { get; set; }
        public string AxItemName { get; set; }

        public string EmailRequester { get; set; }
    }
}
