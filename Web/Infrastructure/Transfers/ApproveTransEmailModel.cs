namespace Web.Infrastructure.Transfers
{
  public class ApproveTransEmailModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int ApproveLevel { get; set; }
    public int FinalApprove { get; set; }
  }
}