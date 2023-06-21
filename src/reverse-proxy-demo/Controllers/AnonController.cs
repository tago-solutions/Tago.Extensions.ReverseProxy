using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cookies_backend.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("anon")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AnonController : ControllerBase
    {

        private readonly ILogger<AnonController> _logger;

        public AnonController(ILogger<AnonController> logger)
        {
            _logger = logger;
        }

        //[Authorize]
        [HttpGet]
        [HttpPost]
        public string Get()
        {
            return "Hi from Anonymous method";
        }
    }
}
