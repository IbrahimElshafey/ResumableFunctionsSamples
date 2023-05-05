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

    }
    public class RequestApprovalWorkflow : ResumableFunction
    {
        private readonly RequestApprovalService service;

        public RequestApprovalWorkflow(RequestApprovalService service)
        {
            this.service = service;
            //passing dependancies in constructor have a bug that I work on
        }

        public Request UserRequest { get; set; }

        [ResumableFunctionEntryPoint("RequestApprovalWorkflow.RequestApprovalFlow")]
        internal async IAsyncEnumerable<Wait> RequestApprovalFlow()
        {
            yield return WaitUserSubmitRequest();
            Console.WriteLine("After User Submit Request");
        }

        private Wait WaitUserSubmitRequest()
        {
            return Wait<Request, bool>("Wait User Submit Request", service.UserSubmitRequest)
                             .MatchIf((request, result) => request.Id > 0)
                             .SetData((request, result) => UserRequest == request);
        }
    }
    public class RequestApprovalService
    {
        public RequestApprovalService()
        {

        }
        [WaitMethod("RequestApproval.UserSubmitRequest")]
        public bool UserSubmitRequest(Request request)
        {
            return true;
        }
    }
    public class Request
    {
        public int Id { get; set; }
        public string SomeContent { get; set; }
    }
}