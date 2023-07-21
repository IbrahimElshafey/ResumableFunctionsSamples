using RequestApproval.Controllers.InOuts;

namespace RequestApproval.Controllers.Service
{
    public interface IRequestApprovalService
    {
        bool SubmitRequest(Request request);
        void ManagerApproval(ApproveRequestArgs input);
    }
}