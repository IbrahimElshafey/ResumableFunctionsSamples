using ResumableFunctions.Handler.Attributes;

namespace RequestApproval.Controllers
{
    public class RequestApprovalService : IRequestApprovalService
    {
        [PushCall("RequestApproval.UserSubmitRequest")]
        public bool UserSubmitRequest(Request request)
        {
            Console.WriteLine($"Request `{request.Id}` submitted.");
            return true;
        }

        public int AskManagerApproval(int requestId)
        {
            Console.WriteLine($"Ask manager to approve request `{requestId}`");
            return requestId + 10;//taskId
        }

        [PushCall("RequestApproval.ManagerApproval")]
        public int ManagerApproval(ApproveRequestArgs input)
        {
            Console.WriteLine($"Manager approval for task `{input.TaskId}`");
            return Random.Shared.Next();//approval id
        }

        public void InformUserAboutAccept(int id)
        {
            //some code
            Console.WriteLine($"Inform user about accept request `{id}`");
        }

        public void InformUserAboutReject(int id)
        {
            //some code
            Console.WriteLine($"Inform user about reject request `{id}`");
        }

        public void AskUserForMoreInfo(int id, string message)
        {
            //some code
            Console.WriteLine($"Ask user for more info about request `{id}`");
        }
    }
}