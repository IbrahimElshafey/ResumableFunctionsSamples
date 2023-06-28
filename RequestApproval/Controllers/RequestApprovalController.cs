using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using ResumableFunctions.Handler;
using ResumableFunctions.Handler.Attributes;
using ResumableFunctions.Handler.InOuts;

namespace RequestApproval.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestApprovalController : ControllerBase
    {
        private readonly ILogger<RequestApprovalController> _logger;
        private readonly RequestApprovalService service;

        public RequestApprovalController(ILogger<RequestApprovalController> logger, RequestApprovalService service)
        {
            _logger = logger;
            this.service = service;
        }

        [HttpPost(nameof(UserSubmitRequest))]
        public bool UserSubmitRequest(Request request)
        {
            return service.UserSubmitRequest(request);
        }

        [HttpPost(nameof(ManagerApproval))]
        public int ManagerApproval(ApproveRequestArgs input)
        {
            return service.ManagerApproval(input);
        }

    }
    public class RequestApprovalWorkflow : ResumableFunction
    {
        private const string WaitSubmitRequest = "Wait User Submit Request";
        private readonly RequestApprovalService service;

        public RequestApprovalWorkflow(RequestApprovalService service)
        {
            this.service = service;
            //passing dependancies in constructor have a bug that I work on
        }

        public Request UserRequest { get; set; }
        public int ManagerApprovalTaskId { get; set; }
        public ApproveRequestArgs ManagerApprovalResult { get; set; }

        [ResumableFunctionEntryPoint("RequestApprovalWorkflow.RequestApprovalFlow")]
        internal async IAsyncEnumerable<Wait> RequestApprovalFlow()
        {
            yield return WaitUserSubmitRequest();
            ManagerApprovalTaskId = service.AskManagerApproval(UserRequest.Id);
            yield return WaitManagerApproval();
            switch (ManagerApprovalResult.Decision)
            {
                case "Accept":
                    service.InformUserAboutAccept(UserRequest.Id);
                    break;
                case "Reject":
                    service.InformUserAboutReject(UserRequest.Id);
                    break;
                case "MoreInfo":
                    service.AskUserForMoreInfo(UserRequest.Id, ManagerApprovalResult.Message);
                    yield return GoBackTo<Request, bool>(WaitSubmitRequest, (request, result) => request.Id == UserRequest.Id);
                    break;
                default: throw new ArgumentException("Allowed values for decision are one of(Accept,Reject,MoreInfo)");
            }
            Console.WriteLine("RequestApprovalFlow Ended");
        }

        private Wait WaitUserSubmitRequest()
        {
            return Wait<Request, bool>(WaitSubmitRequest, service.UserSubmitRequest)
                    .MatchIf((request, result) => request.Id > 0)
                    .SetData((request, result) => UserRequest == request);
        }

        private Wait WaitManagerApproval()
        {
            return Wait<ApproveRequestArgs, int>("Wait Manager Approval", service.ManagerApproval)
                    .MatchIf((approveRequestArgs, approvalId) => approvalId > 0 && approveRequestArgs.TaskId == ManagerApprovalTaskId)
                    .SetData((approveRequestArgs, approvalId) => ManagerApprovalResult == approveRequestArgs);
        }
    }
    public class RequestApprovalService
    {
        public RequestApprovalService()
        {

        }

        [PushCall("RequestApproval.UserSubmitRequest")]
        public bool UserSubmitRequest(Request request)
        {
            return true;
        }

        public int AskManagerApproval(int requestId)
        {
            var taskId = Random.Shared.Next() + requestId;
            return taskId;
        }

        [PushCall("RequestApproval.ManagerApproval")]
        public int ManagerApproval(ApproveRequestArgs input)
        {
            var approvalId = Random.Shared.Next();
            return approvalId;
        }

        internal void InformUserAboutAccept(int id)
        {
            //some code
        }

        internal void InformUserAboutReject(int id)
        {
            //some code
        }

        internal void AskUserForMoreInfo(int id, string message)
        {
            //some code
        }
    }
    public class Request
    {
        public int Id { get; set; }
        public string SomeContent { get; set; }
    }

    public class ApproveRequestArgs
    {
        public int TaskId { get; set; }
        public string Decision { get; set; } = "Accept";
        public string Message { get; set; }
    }
}