using Microsoft.AspNetCore.Mvc;

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

    public class RequestApprovalService : IRequestApprovalService
    {
        private static List<Request> requests = new();

        public bool SubmitRequest(Request request)
        {
            if (requests.Any(x => x.Id == request.Id))
            {
                //Must be in edit mode
                Console.WriteLine($"Request `{request.Id}` re-submitted.");
                //Update request
            }
            else
            {
                Console.WriteLine($"Request `{request.Id}` submitted.");
                requests.Add(request);
            }
            request.ManagerApprovalTaskId = AskManagerApproval(request.Id);
            return true;
        }

        private int AskManagerApproval(int requestId)
        {
            Console.WriteLine($"Ask manager to approve request `{requestId}`");
            return requestId + 10;//taskId
        }

        public void ManagerApproval(ApproveRequestArgs input)
        {
            Console.WriteLine($"Manager approval for task `{input.TaskId}`");
            var request = requests.First(x => x.ManagerApprovalTaskId == input.TaskId);
            switch (input.Decision)
            {
                case "Accept":
                    InformUserAboutAccept(request.Id);
                    Console.WriteLine("RequestApprovalFlow Ended");//this line may be another process
                    break;
                case "Reject":
                    InformUserAboutReject(request.Id);
                    Console.WriteLine("RequestApprovalFlow Ended");//this line may be another process
                    break;
                case "MoreInfo":
                    AskUserForMoreInfo(request.Id, input.Message);
                    break;
                default:
                    throw new ArgumentException("Allowed values for decision are one of(Accept,Reject,MoreInfo)");
            }

        }

        private void InformUserAboutAccept(int id)
        {
            //some code
            Console.WriteLine($"Inform user about accept request `{id}`");
        }

        private void InformUserAboutReject(int id)
        {
            //some code
            Console.WriteLine($"Inform user about reject request `{id}`");
        }

        private void AskUserForMoreInfo(int id, string message)
        {
            Console.WriteLine($"Ask user for more info about request `{id}`");
            //Send a message to the request owner
            //some other code
        }
    }
    public class Request
    {
        public int Id { get; set; }
        public string SomeContent { get; set; }
        public int ManagerApprovalTaskId { get; set; }
        //public bool CanUserEdit { get; set; }
    }

    public class ApproveRequestArgs
    {
        public int TaskId { get; set; }
        public string Decision { get; set; } = "Accept";
        public string Message { get; set; }
    }
}