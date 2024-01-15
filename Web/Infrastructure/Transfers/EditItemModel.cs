using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
    public class EditItemModel
    {
        public int EId { get; set; }
        public string EItemGroup { get; set; }
        public string ESubGroup { get; set; }
        public string EUnit { get; set; }
        public string EProductGroup { get; set; }
        public string EItemName { get; set; }
        public string EItemPart { get; set; }
        public string EItemModel { get; set; }
        public string EItemDescription { get; set; }
        public string EItemBrand { get; set; }
        public string ERemark { get; set; }
        public int UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

    }
}
