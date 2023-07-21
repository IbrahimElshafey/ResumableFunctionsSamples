namespace RequestApproval.Controllers.InOuts
{
    public class Request
    {
        public int Id { get; set; }
        public string SomeContent { get; set; }
        public int ManagerApprovalTaskId { get; set; }
        //public bool CanUserEdit { get; set; }
    }
}