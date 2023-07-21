using ResumableFunctions.Handler;
using ResumableFunctions.Handler.Attributes;
using ResumableFunctions.Handler.InOuts;

namespace RequestApproval.Controllers
{
    public class RequestApprovalWorkflow : ResumableFunctionsContainer
    {
        private const string WaitSubmitRequest = "Wait User Submit Request";
        private IRequestApprovalService _service;

        public void SetDependencies(IRequestApprovalService service)
        {
            _service = service;
        }

        public Request UserRequest { get; set; }
        public int ManagerApprovalTaskId { get; set; }
        public ApproveRequestArgs ManagerApprovalResult { get; set; }



        [ResumableFunctionEntryPoint("RequestApprovalWorkflow.RequestApprovalFlow")]
        internal async IAsyncEnumerable<Wait> RequestApprovalFlow()
        {
            yield return WaitUserSubmitRequest();

            if (ManagerApprovalTaskId != default)
                Console.WriteLine($"Request `{UserRequest.Id}` re-submitted.");

            ManagerApprovalTaskId = _service.AskManagerApproval(UserRequest.Id);
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
                    yield return GoBackTo<Request, bool>(WaitSubmitRequest, (request, result) => request.Id == UserRequest.Id);
                    break;
                default: throw new ArgumentException("Allowed values for decision are one of(Accept,Reject,MoreInfo)");
            }

            await Task.Delay(100);
            Console.WriteLine("RequestApprovalFlow Ended");
        }

        private Wait WaitUserSubmitRequest()
        {
            return Wait<Request, bool>(_service.UserSubmitRequest, WaitSubmitRequest)
                    .MatchIf((request, result) => request.Id > 0)
                    .SetData((request, result) => UserRequest == request);
        }

        private Wait WaitManagerApproval()
        {
            return Wait<ApproveRequestArgs, int>(_service.ManagerApproval, "Wait Manager Approval")
                    .MatchIf((approveRequestArgs, approvalId) => approvalId > 0 && approveRequestArgs.TaskId == ManagerApprovalTaskId)
                    .SetData((approveRequestArgs, approvalId) => ManagerApprovalResult == approveRequestArgs);
        }
    }
}