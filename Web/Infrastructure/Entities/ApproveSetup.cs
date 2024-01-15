using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Infrastructure.Entities
{
    [Table("ApproveSetup")]
    public class ApproveSetup
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ApproveGroup { get; set; }
        public int ApproveLevel { get; set; }
        public int NotifyTo { get; set; }
        public int FinalApprove { get; set; }
        public int CanApprove { get; set; }
        public int Active { get; set; }

        [EmailAddress]
        public string EmailBackup { get; set; }
    }
}