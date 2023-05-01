using ClientOnboarding.InOuts;
using ClientOnboarding.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientOnboarding.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientOnboardingController : ControllerBase
    {


        private readonly ILogger<ClientOnboardingController> _logger;
        private readonly ClientOnboardingService service;

        public ClientOnboardingController(ILogger<ClientOnboardingController> logger, ClientOnboardingService service)
        {
            _logger = logger;
            this.service = service;
        }

        [HttpPost(nameof(ClientFillsForm))]
        public RegistrationResult ClientFillsForm(RegistrationForm registrationForm)
        {
            return service.ClientFillsForm(registrationForm);
        }
    }
}