namespace Web.Infrastructure.Transfers
{
    public class ApproveSetupGridModel
    {
        public int Id { get; set; }
        public string Employee { get; set; }
        public string Email { get; set; }
        public string ApproveGroup { get; set; }
        public int ApproveLevel { get; set; }
        public int NotifyTo { get; set; }
        public int FinalApprove { get; set; }
        public int CanApprove { get; set; }
        public string EmailBackup { get; set; }
        public int Active { get; set; }
        public string Company { get; set; }
    }
}