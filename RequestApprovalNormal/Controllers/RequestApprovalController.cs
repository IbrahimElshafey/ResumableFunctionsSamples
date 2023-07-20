using Microsoft.AspNetCore.Mvc;

namespace RequestApproval.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestApprovalController : ControllerBase
    {
        private readonly ILogger<RequestApprovalController> _logger;
        private readonly RequestApprovalService _service;
        public RequestApprovalController(ILogger<RequestApprovalController> logger, RequestApprovalService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost(nameof(UserSubmitRequest))]
        public bool UserSubmitRequest(Request request)
        {
            if (_service.SubmitRequest(request))
            {
                request.ManagerApprovalTaskId = _service.AskManagerApproval(request.Id);
            }
            return false;
        }

        [HttpPost(nameof(ManagerApproval))]
        public int ManagerApproval(ApproveRequestArgs input)
        {
            _service.ManagerApproval(input);
            return 1;
        }

    }
    
    public class RequestApprovalService
    {
        private static List<Request> requests = new();

        public bool SubmitRequest(Request request)
        {
            Console.WriteLine($"Request `{request.Id}` submitted.");
            if(request.InEditMode)
            {
                request.ManagerApprovalTaskId = AskManagerApproval(request.Id);
            }
            else
            {
                requests.Add(request);
            }
            return true;
        }

        public int AskManagerApproval(int requestId)
        {
            Console.WriteLine($"Ask manager to approve request `{requestId}`");
            return Random.Shared.Next() + requestId;//taskId
        }

        public void ManagerApproval(ApproveRequestArgs input)
        {
            Console.WriteLine($"Manager approval for task `{input.TaskId}`");
            var request = requests.First(x => x.ManagerApprovalTaskId == input.TaskId);
            switch (input.Decision)
            {
                case "Accept":
                    InformUserAboutAccept(request.Id);
                    break;
                case "Reject":
                    InformUserAboutReject(request.Id);
                    break;
                case "MoreInfo":
                    AskUserForMoreInfo(request.Id, input.Message);
                    break;
                default:
                    throw new ArgumentException("Allowed values for decision are one of(Accept,Reject,MoreInfo)");
            }
            Console.WriteLine("RequestApprovalFlow Ended");//this line may be another process
        }

        internal void InformUserAboutAccept(int id)
        {
            //some code
            Console.WriteLine($"Inform user about accept request `{id}`");
        }

        internal void InformUserAboutReject(int id)
        {
            //some code
            Console.WriteLine($"Inform user about reject request `{id}`");
        }

        internal void AskUserForMoreInfo(int id, string message)
        {
            Console.WriteLine($"Ask user for more info about request `{id}`");
            //Send a message to the request owner
            //Set request in edit mode
            var request = requests.First(x => x.Id == id);
            request.InEditMode = true;
            //some other code
        }
    }
    public class Request
    {
        public int Id { get; set; }
        public string SomeContent { get; set; }
        public int ManagerApprovalTaskId { get; set; }
        public bool InEditMode { get; internal set; }
    }

    public class ApproveRequestArgs
    {
        public int TaskId { get; set; }
        public string Decision { get; set; } = "Accept";
        public string Message { get; set; }
    }
}