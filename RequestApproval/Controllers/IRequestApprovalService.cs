using ResumableFunctions.Handler.Attributes;

namespace RequestApproval.Controllers
{
    public interface IRequestApprovalService
    {
        bool UserSubmitRequest(Request request);
        int AskManagerApproval(int requestId);
        int ManagerApproval(ApproveRequestArgs input);
        void InformUserAboutAccept(int id);
        void InformUserAboutReject(int id);
        void AskUserForMoreInfo(int id, string message);
    }
}