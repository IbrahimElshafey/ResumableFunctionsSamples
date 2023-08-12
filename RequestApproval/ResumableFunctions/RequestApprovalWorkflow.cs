using ResumableFunctions.Handler;
using ResumableFunctions.Handler.Attributes;
using ResumableFunctions.Handler.BaseUse;

namespace RequestApproval.Controllers
{
    public class RequestApprovalWorkflow : ResumableFunctionsContainer
    {
        [ResumableFunctionEntryPoint("RequestApprovalWorkflow.RequestApprovalFlow")]
        internal async IAsyncEnumerable<Wait> RequestApprovalFlow()
        {
            yield return WaitUserSubmitRequest();

            yield return WaitManagerApproval();

            switch (ManagerApprovalResult.Decision)
            {
                case "Accept":
                    _service.InformUserAboutAccept(UserRequest.Id);
                    break;
                case "Reject":
                    _service.InformUserAboutReject(UserRequest.Id);
                    break;
                case "MoreInfo":
                    _service.AskUserForMoreInfo(UserRequest.Id, ManagerApprovalResult.Message);
                    yield return GoBackTo<Request, bool>(WaitSubmitRequest,
                            (request, result) => request.Id == UserRequest.Id);
                    break;
                default:
                    throw new ArgumentException("Allowed values for decision are one of(Accept,Reject,MoreInfo)");
            }

            await Task.Delay(100);
            Console.WriteLine("RequestApprovalFlow Ended");
        }
        private const string WaitSubmitRequest = "Wait User Submit Request";
        private IRequestApprovalService _service;

        public void SetDependencies(IRequestApprovalService service)
        {
            _service = service;
        }

        public Request UserRequest { get; set; }
        public int ManagerApprovalTaskId { get; set; }
        public ApproveRequestArgs ManagerApprovalResult { get; set; }



        

        private Wait WaitUserSubmitRequest()
        {
            return Wait<Request, bool>(_service.UserSubmitRequest, WaitSubmitRequest)
                    .MatchIf((request, result) => request.Id > 0)
                    .AfterMatch((request, result) => UserRequest = request);
        }

        private Wait WaitManagerApproval()
        {
            ManagerApprovalTaskId = _service.AskManagerApproval(UserRequest.Id);
            return Wait<ApproveRequestArgs, int>(_service.ManagerApproval, "Wait Manager Approval")
                    .MatchIf((approveRequestArgs, approvalId) => approvalId > 0 && approveRequestArgs.TaskId == ManagerApprovalTaskId)
                    .AfterMatch((approveRequestArgs, approvalId) => ManagerApprovalResult = approveRequestArgs);
        }
    }
}