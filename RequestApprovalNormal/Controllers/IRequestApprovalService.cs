namespace RequestApproval.Controllers
{
    public interface IRequestApprovalService
    {
        bool SubmitRequest(Request request);
        void ManagerApproval(ApproveRequestArgs input);
    }
}