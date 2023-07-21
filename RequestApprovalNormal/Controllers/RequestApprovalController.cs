using Microsoft.AspNetCore.Mvc;
using RequestApproval.Controllers.InOuts;
using RequestApproval.Controllers.Service;

namespace RequestApproval.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestApprovalController : ControllerBase
    {
        private readonly ILogger<RequestApprovalController> _logger;
        private readonly IRequestApprovalService _service;
        public RequestApprovalController(ILogger<RequestApprovalController> logger, IRequestApprovalService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost(nameof(UserSubmitRequest))]
        public bool UserSubmitRequest(Request request)
        {
            return _service.SubmitRequest(request);
        }

        [HttpPost(nameof(ManagerApproval))]
        public int ManagerApproval(ApproveRequestArgs input)
        {
            _service.ManagerApproval(input);
            return 1;
        }

    }
}