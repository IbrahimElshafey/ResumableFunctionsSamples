using ClientOnboarding.InOuts;
using ClientOnboarding.Services;
using ResumableFunctions.Handler;
using ResumableFunctions.Handler.Attributes;
using ResumableFunctions.Handler.BaseUse;

namespace ClientOnboarding.Workflow
{
    public class ClientOnboardingWorkflow : ResumableFunctionsContainer
    {
        private  IClientOnboardingService _service;

        public void SetDependencies(IClientOnboardingService service)
        {
            _service = service;
        }

        [ResumableFunctionEntryPoint("ClientOnboardingWorkflow.StartClientOnboardingWorkflow")]
        internal async IAsyncEnumerable<Wait> StartClientOnboardingWorkflow()
        {
            yield return WaitUserRegistration();

            yield return OwnerApproveClient();

            if (OwnerApprovalInput.Decision is false)
            {
                _service.InformUserAboutRejection(RegistrationForm.UserId);
            }
            else if (OwnerApprovalInput.Decision is true)
            {
                _service.SendWelcomePackage(RegistrationForm.UserId);

                yield return WaitMeetingResult();
                Console.WriteLine(MeetingResult);
            }

            Console.WriteLine("Client Onboarding Workflow Done");
        }

        private MethodWait<RegistrationForm, RegistrationResult> WaitUserRegistration()
        {
            return 
                Wait<RegistrationForm, RegistrationResult>(_service.ClientFillsForm, "Wait User Registration")
                .MatchIf((regForm, regResult) => regResult.FormId > 0)
                .AfterMatch((regForm, regResult) =>
                {
                    RegistrationForm = regForm;
                    RegistrationResult = regResult;
                });
        }

        private MethodWait<OwnerApproveClientInput, OwnerApproveClientResult> OwnerApproveClient()
        {
            OwnerTaskId = _service.AskOwnerToApproveClient(RegistrationResult.FormId);
            return 
                Wait<OwnerApproveClientInput, OwnerApproveClientResult>(
                        _service.OwnerApproveClient, "Wait Owner Approve Client")
                .MatchIf((approveClientInput, approveResult) => approveClientInput.TaskId == OwnerTaskId.Id)
                .AfterMatch((approveClientInput, approveResult) =>
                {
                    OwnerTaskResult = approveResult;
                    OwnerApprovalInput = approveClientInput;
                });
        }

        private MethodWait<int, MeetingResult> WaitMeetingResult()
        {
            ClientMeetingId = _service.SetupInitalMeetingAndAgenda(RegistrationForm.UserId);
            return
                Wait<int, MeetingResult>(_service.SendMeetingResult, "Wait Meeting Result")
                .MatchIf((mmetingId, meetingResult) => mmetingId == ClientMeetingId.MeetingId)
                .AfterMatch((mmetingId, meetingResult) => MeetingResult = meetingResult);
        }

        public RegistrationForm RegistrationForm { get; set; }
        public RegistrationResult RegistrationResult { get; set; }
        public TaskId OwnerTaskId { get; set; }
        public OwnerApproveClientResult OwnerTaskResult { get; set; }
        public ClientMeetingId ClientMeetingId { get; set; }
        public MeetingResult MeetingResult { get; set; }
        public OwnerApproveClientInput OwnerApprovalInput { get; set; }
    }
}
